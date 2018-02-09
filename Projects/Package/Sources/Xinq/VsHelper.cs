using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Xinq
{
    public static class VsHelper
    {
        public static IVsHierarchy ToHierarchy(EnvDTE.Project project)
        {
            if (project == null) throw new ArgumentNullException("project"); string projectGuid = null;        // DTE does not expose the project GUID that exists at in the msbuild project file.        // Cannot use MSBuild object model because it uses a static instance of the Engine,         // and using the Project will cause it to be unloaded from the engine when the         // GC collects the variable that we declare.       
            using (XmlReader projectReader = XmlReader.Create(project.FileName))
            {
                projectReader.MoveToContent();
                object nodeName = projectReader.NameTable.Add("ProjectGuid");
                while (projectReader.Read())
                {
                    if (Object.Equals(projectReader.LocalName, nodeName))
                    {
                        projectGuid = (String)projectReader.ReadElementContentAsString(); break;
                    }
                }
            }
            Debug.Assert(!String.IsNullOrEmpty(projectGuid));
            IServiceProvider serviceProvider = new ServiceProvider(project.DTE as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
            return VsShellUtilities.GetHierarchy(serviceProvider, new Guid(projectGuid));
        }

        public static IVsProject ToVsProject(EnvDTE.Project project)
        {
            if (project == null) throw new ArgumentNullException("project");
            IVsProject vsProject = ToHierarchy(project) as IVsProject;
            if (vsProject == null)
            {
                throw new ArgumentException("Project is not a VS project.");
            }
            return vsProject;
        }
    }
}