using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj80;

namespace Xinq
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0.7", IconResourceID = 400)]
    [ProvideXmlEditorChooserDesignerView("Xinq Designer", "xinq", LogicalViewID.Designer, 0x60, DesignerLogicalViewEditor = typeof(XinqEditorFactory), Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005", MatchExtensionAndNamespace = false)]
    [ProvideEditorLogicalView(typeof(XinqEditorFactory), LogicalViewID.Designer)]
    [ProvideEditorExtension(typeof(XinqEditorFactory), ".xinq", 32, ProjectGuid = vsContextGuids.vsContextGuidVCSProject, NameResourceID = 105)]
    [ProvideEditorExtension(typeof(XinqEditorFactory), ".xinq", 32, ProjectGuid = vsContextGuids.vsContextGuidVBProject, NameResourceID = 105)]
    [Guid(GuidList.XinqPackageGuidString)]
    internal sealed class XinqPackage : AsyncPackage, IVsInstalledProduct
    {
        private XinqEditorFactory _editorFactory;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            base.Initialize();

            _editorFactory = new XinqEditorFactory(this);
            RegisterEditorFactory(_editorFactory);

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        public string GetResourceString(uint id)
        {
            var vsShell = GetService(typeof(SVsShell)) as IVsShell;

            var guidPackage = GuidList.XinqPackageGuid;
            string pbstrOut;

            var hr = vsShell.LoadPackageString(ref guidPackage, id, out pbstrOut);
            ErrorHandler.ThrowOnFailure(hr);

            return pbstrOut;
        }

        #region "IVsInstalledProduct Members"

        public int IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 0;

            return VSConstants.S_OK;
        }

        public int IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = 401;

            return VSConstants.S_OK;
        }

        public int OfficialName(out string pbstrName)
        {
            pbstrName = GetResourceString(110);

            return VSConstants.S_OK;
        }

        public int ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails = GetResourceString(112);

            return VSConstants.S_OK;
        }

        public int ProductID(out string pbstrPID)
        {
            pbstrPID = GetResourceString(111);

            return VSConstants.S_OK;
        }

        #endregion
    }
}
