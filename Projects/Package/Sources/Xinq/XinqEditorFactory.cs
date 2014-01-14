using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Xinq
{
    [Guid(GuidList.XinqEditorFactoryGuidString)]
    internal class XinqEditorFactory : EditorFactory
    {
        private XinqPackage _package;
        private ServiceProvider _serviceProvider;

        public XinqEditorFactory(XinqPackage package)
        {
            _package = package;
        }

        public override int MapLogicalView(ref Guid logicalView, out string physicalView)
        {
            physicalView = null;

            // Requested when:
            // - VS opens a project and the previously opened file has to be displayed
            // - The 'Open With' dialog is about to be displayed
            // - The View | Open command
            if (logicalView == VSConstants.LOGVIEWID_Primary)
            {
                // Primary view uses NULL as physicalView
                return VSConstants.S_OK;
            }

            if (logicalView == VSConstants.LOGVIEWID_Designer)
            {
                physicalView = "Designer";
                return VSConstants.S_OK;
            }

            return VSConstants.E_NOTIMPL;
        }

        public override int CreateEditorInstance(uint createDocFlags, string moniker, string physicalView, IVsHierarchy pHier, uint itemid, IntPtr existingDocData, out IntPtr docView, out IntPtr docData, out string editorCaption, out Guid cmdUI, out int cancelled)
        {
            docView = IntPtr.Zero;
            docData = IntPtr.Zero;
            editorCaption = null;
            cmdUI = GuidList.XinqEditorFactoryGuid;
            cancelled = 1;

            if ((createDocFlags & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
                return VSConstants.E_INVALIDARG;

            // Prompt user to close the currently open editor
            if (existingDocData != IntPtr.Zero)
                return VSConstants.VS_E_INCOMPATIBLEDOCDATA;

            var editor = new XinqEditorPane(_package);
            docView = Marshal.GetIUnknownForObject(editor);
            docData = Marshal.GetIUnknownForObject(editor);
            editorCaption = _package.GetResourceString(113);
            cancelled = 0;

            return VSConstants.S_OK;
        }

        public override int SetSite(IOleServiceProvider psp)
        {
            _serviceProvider = new ServiceProvider(psp);

            return base.SetSite(psp);
        }
    }
}
