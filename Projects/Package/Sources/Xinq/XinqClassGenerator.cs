using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using VSLangProj80;
using CodeNamespace = System.CodeDom.CodeNamespace;

namespace Xinq
{
    [CodeGeneratorRegistration(typeof(XinqClassGenerator), "C# Xinq Class Generator", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
    [CodeGeneratorRegistration(typeof(XinqClassGenerator), "VB Xinq Class Generator", vsContextGuids.vsContextGuidVBProject, GeneratesDesignTimeSource = true)]
    [ProvideObject(typeof(XinqClassGenerator))]
    [Guid(GuidList.XinqSingleFileGeneratorGuidString)]
    internal class XinqClassGenerator : BaseCodeGeneratorWithSite
    {
        protected override string GetDefaultExtension()
        {
            return ".Designer" + base.GetDefaultExtension();
        }

        protected override byte[] GenerateCode(string inputFileContent)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);

            var document = new XinqDocument();
            document.Load(InputFilePath);

            var generatorOptions = new CodeGeneratorOptions();
            var codeProivder = GetCodeProvider();

            var queryBaseName = Path.GetFileNameWithoutExtension(InputFilePath) + ".";
            var inputFileDir = InputFilePath.Replace(Path.GetFileName(InputFilePath), "");
            var defaultExtension = GetDefaultExtension();
            var candidateFilesToDelete = Directory.GetFiles(inputFileDir).Select(Path.GetFileName).Where(t => t.StartsWith(queryBaseName) && t.EndsWith(defaultExtension)).ToList();

            int iFound = 0;
            uint itemId = 0;
            EnvDTE.ProjectItem item;
            Microsoft.VisualStudio.Shell.Interop.VSDOCUMENTPRIORITY[] pdwPriority = new Microsoft.VisualStudio.Shell.Interop.VSDOCUMENTPRIORITY[1];

            EnvDTE.DTE dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
            Array ary = (Array)dte.ActiveSolutionProjects;
            Project project = null;
            if (ary.Length > 0)
            {
                project = (EnvDTE.Project)ary.GetValue(0);
            }
            else
            {
                project = dte.Solution.Projects.Item(1);
            }

            // obtain a reference to the current project as an IVsProject type
            Microsoft.VisualStudio.Shell.Interop.IVsProject VsProject = VsHelper.ToVsProject(project);
            // this locates, and returns a handle to our source file, as a ProjectItem
            VsProject.IsDocumentInProject(InputFilePath, out iFound, pdwPriority, out itemId);


            // if our source file was found in the project (which it should have been)
            if (iFound != 0 && itemId != 0)
            {
                Microsoft.VisualStudio.OLE.Interop.IServiceProvider oleSp = null;
                VsProject.GetItemContext(itemId, out oleSp);
                if (oleSp != null)
                {
                    ServiceProvider sp = new ServiceProvider(oleSp);
                    // convert our handle to a ProjectItem
                    item = sp.GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
                }
                else
                    throw new ApplicationException("Unable to retrieve Visual Studio ProjectItem");
            }
            else
                throw new ApplicationException("Unable to retrieve Visual Studio ProjectItem");

            foreach (var query in document.Queries)
            {
                var queryFileName = queryBaseName + query.Name + defaultExtension;
                var queryFilePath = Path.Combine(inputFileDir, queryFileName);
                candidateFilesToDelete.Remove(queryFileName);
                using (var fileStream = new FileStream(queryFilePath, FileMode.Create))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    var compileUnit = GenerateCode(query);
                    codeProivder.GenerateCodeFromCompileUnit(compileUnit, streamWriter, generatorOptions);
                    streamWriter.Flush();
                    // add the newly generated file to the solution, as a child of the source file...
                    EnvDTE.ProjectItem itm = item.ProjectItems.AddFromFile(queryFilePath);
                }
            }

            // perform some clean-up, making sure we delete any old (stale) target-files
            if (candidateFilesToDelete.Count != 0)
            {
                candidateFilesToDelete.ForEach(t =>
                {
                    foreach (EnvDTE.ProjectItem childItem in item.ProjectItems)
                    {
                        if (childItem.Name == t)
                            // then delete it
                            childItem.Delete();
                    }

                    var fp = Path.Combine(inputFileDir, t);
                    if (File.Exists(fp))
                        File.Delete(fp);
                });
            }

            var mainClass = GenerateCode(null);
            codeProivder.GenerateCodeFromCompileUnit(mainClass, writer, generatorOptions);
            writer.Flush();

            return stream.ToArray();
        }

        public CodeCompileUnit GenerateCode(Query query)
        {
            var codeUnit = new CodeCompileUnit();
            codeUnit.UserData.Add("AllowLateBound", false);
            codeUnit.UserData.Add("RequireVariableDeclaration", true);

            var namespaceCode = new CodeNamespace();
            namespaceCode.Name = !string.IsNullOrEmpty(FileNameSpace) ? FileNameSpace : null;
            namespaceCode.Imports.Add(new CodeNamespaceImport("System"));
            codeUnit.Namespaces.Add(namespaceCode);

            var mainClassCode = new CodeTypeDeclaration();
            mainClassCode.IsClass = true;
            mainClassCode.IsPartial = true;
            mainClassCode.Name = GetValidLanguageIndependentIdentifier(Path.GetFileNameWithoutExtension(InputFilePath));
            mainClassCode.Attributes = MemberAttributes.Static;
            mainClassCode.TypeAttributes = TypeAttributes.NotPublic;

            if (query != null)
            {
                var queryName = GetValidLanguageIndependentIdentifier(query.Name);

                var fieldCode = new CodeMemberField();
                fieldCode.Name = string.Format("_{0}{1}", queryName.Substring(0, 1).ToLower(), queryName.Substring(1));

                fieldCode.Type = new CodeTypeReference("Query");
                fieldCode.Attributes = MemberAttributes.Private | MemberAttributes.Static;
                fieldCode.InitExpression = new CodeObjectCreateExpression("Query",
                    new CodePrimitiveExpression(query.NormalizedText));
                mainClassCode.Members.Add(fieldCode);

                var propertyCode = new CodeMemberProperty();
                propertyCode.Name = queryName;
                propertyCode.Type = new CodeTypeReference("Query");
                propertyCode.Attributes = MemberAttributes.Assembly | MemberAttributes.Final | MemberAttributes.Static;

                var commentCode =
                    new CodeCommentStatement(
                        string.Format("<summary>{1} {0}{1} </summary>", query.Comment, Environment.NewLine), true);
                propertyCode.Comments.Add(commentCode);

                var returnCode = new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, fieldCode.Name));
                propertyCode.GetStatements.Add(returnCode);

                mainClassCode.Members.Add(propertyCode);
            }
            else
            {
                var generatedCodeAttributeCode = new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression("XinqClassGenerator")), new CodeAttributeArgument(new CodePrimitiveExpression(GetType().Assembly.GetName().Version.ToString())));
                generatedCodeAttributeCode.AttributeType.Options = CodeTypeReferenceOptions.GlobalReference;
                mainClassCode.CustomAttributes.Add(generatedCodeAttributeCode);

                var debuggerNonUserCodeAttributeCode = new CodeAttributeDeclaration(new CodeTypeReference(typeof(DebuggerNonUserCodeAttribute)));
                debuggerNonUserCodeAttributeCode.AttributeType.Options = CodeTypeReferenceOptions.GlobalReference;
                mainClassCode.CustomAttributes.Add(debuggerNonUserCodeAttributeCode);

                var compilerGeneratedAttributeCode = new CodeAttributeDeclaration(new CodeTypeReference(typeof(CompilerGeneratedAttribute)));
                compilerGeneratedAttributeCode.AttributeType.Options = CodeTypeReferenceOptions.GlobalReference;
                mainClassCode.CustomAttributes.Add(compilerGeneratedAttributeCode);

                mainClassCode.Members.Add(GenerateQueryClassCode());
            }

            namespaceCode.Types.Add(mainClassCode);

            return codeUnit;
        }

        private CodeTypeDeclaration GenerateQueryClassCode()
        {
            var queryClassCode = new CodeTypeDeclaration();
            queryClassCode.IsClass = true;
            queryClassCode.Name = "Query";
            queryClassCode.TypeAttributes = TypeAttributes.NotPublic;

            var editorBrowsableAttributeCode = new CodeAttributeDeclaration(new CodeTypeReference(typeof(EditorBrowsableAttribute)), new CodeAttributeArgument(new CodeSnippetExpression("global::System.ComponentModel.EditorBrowsableState.Never")));
            editorBrowsableAttributeCode.AttributeType.Options = CodeTypeReferenceOptions.GlobalReference;
            queryClassCode.CustomAttributes.Add(editorBrowsableAttributeCode);

            var textFieldCode = new CodeMemberField();
            textFieldCode.Name = "_text";
            textFieldCode.Type = new CodeTypeReference(typeof(string));
            textFieldCode.Attributes = MemberAttributes.Private;
            queryClassCode.Members.Add(textFieldCode);

            var ctorCode = new CodeConstructor();
            ctorCode.Attributes = MemberAttributes.Public;
            ctorCode.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "text"));
            ctorCode.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(null, "_text"), new CodeArgumentReferenceExpression("text")));
            queryClassCode.Members.Add(ctorCode);

            var textPropertyCode = new CodeMemberProperty();
            textPropertyCode.Name = "Text";
            textPropertyCode.Type = new CodeTypeReference(typeof(string));
            textPropertyCode.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            textPropertyCode.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, "_text")));

            var commentCode = new CodeCommentStatement(string.Format("<summary>{0} Gets query's text.{0} </summary>", Environment.NewLine), true);
            textPropertyCode.Comments.Add(commentCode);

            queryClassCode.Members.Add(textPropertyCode);

            return queryClassCode;
        }

        private static string GetValidLanguageIndependentIdentifier(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("value");

            if (CodeGenerator.IsValidLanguageIndependentIdentifier(value))
                return value;

            var characters = value.ToCharArray();

            var stringBuilder = new StringBuilder();

            if (char.GetUnicodeCategory(characters[0]) == UnicodeCategory.DecimalDigitNumber)
                stringBuilder.Append('_');

            foreach (var c in characters)
            {
                switch (char.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.TitlecaseLetter:
                    case UnicodeCategory.ModifierLetter:
                    case UnicodeCategory.OtherLetter:
                    case UnicodeCategory.NonSpacingMark:
                    case UnicodeCategory.SpacingCombiningMark:
                    case UnicodeCategory.DecimalDigitNumber:
                    case UnicodeCategory.ConnectorPunctuation:
                        stringBuilder.Append(c);
                        break;

                    default:
                        stringBuilder.Append('_');
                        break;
                }
            }

            var text = stringBuilder.ToString();

            if (!CodeGenerator.IsValidLanguageIndependentIdentifier(text))
                throw new ArgumentException("value");

            return text;
        }
    }
}
