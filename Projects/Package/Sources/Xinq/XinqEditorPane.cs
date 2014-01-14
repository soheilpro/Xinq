using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Xinq
{
    internal class XinqEditorPane : WindowPane, IVsPersistDocData2, IPersistFileFormat, IVsFileChangeEvents, IVsDocDataFileChangeControl, IOleCommandTarget, IVsWindowPaneCommit
    {
        private XinqPackage _package;
        private XinqEditor _editor;
        private XinqDocument _document;
        private uint _documentCookie;
        private uint _ignoreFileChangeLevel;
        private uint _vsFileChangeCookie = VSConstants.VSCOOKIE_NIL;
        private bool _canEditGuard;
        private const int WM_ACTIVATEAPP = 0x1C;

        public XinqEditorPane(XinqPackage package) : base(package)
        {
            _package = package;
            _editor = new XinqEditor(package, this);
        }

        public override IWin32Window Window
        {
            get
            {
                return _editor;
            }
        }

        public bool CanEdit()
        {
            if (_canEditGuard)
                return false;

            _canEditGuard = true;

            uint pfEditVerdict; // tagVSQueryEditResult
            uint prgfMoreInfo; // tagVSQueryEditResultFlags

            var vsQueryEditQuerySave = GetService(typeof(SVsQueryEditQuerySave)) as IVsQueryEditQuerySave2;

            string[] documents = {
                _document.Filename
            };
            var rgrgf = new uint[0];

            var hr = vsQueryEditQuerySave.QueryEditFiles(0, documents.Length, documents, rgrgf, null, out pfEditVerdict, out prgfMoreInfo);
            ErrorHandler.ThrowOnFailure(hr);

            bool canEdit;

            switch (pfEditVerdict)
            {
                case (uint)tagVSQueryEditResult.QER_EditOK:
                    canEdit = true;
                    break;

                case (uint)tagVSQueryEditResult.QER_EditNotOK:
                    canEdit = false;
                    break;

                default:
                    throw new NotSupportedException();
            }

            _canEditGuard = false;

            return canEdit;
        }

        private void AdviseFileChanges(string filename)
        {
            if (_vsFileChangeCookie != VSConstants.VSCOOKIE_NIL)
                return;

            var vsFileChangeEx = GetService(typeof(SVsFileChangeEx)) as IVsFileChangeEx;

            var hr = vsFileChangeEx.AdviseFileChange(filename, (uint)(_VSFILECHANGEFLAGS.VSFILECHG_Attr | _VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time), this, out _vsFileChangeCookie);
            ErrorHandler.ThrowOnFailure(hr);
        }

        private void UnadviseFileChanges()
        {
            if (_vsFileChangeCookie == VSConstants.VSCOOKIE_NIL)
                return;

            var vsFileChangeEx = GetService(typeof(SVsFileChangeEx)) as IVsFileChangeEx;

            var hr = vsFileChangeEx.UnadviseFileChange(_vsFileChangeCookie);
            ErrorHandler.ThrowOnFailure(hr);

            _vsFileChangeCookie = VSConstants.VSCOOKIE_NIL;
        }

        protected override bool PreProcessMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ACTIVATEAPP:
                    // TODO: Prompt for reloading any changed files
                    break;
            }

            return base.PreProcessMessage(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_editor != null)
                    _editor.Dispose();
            }

            base.Dispose(disposing);
        }

        #region IVsPersistDocData2 Members

        public int Close()
        {
            UnadviseFileChanges();
            _documentCookie = 0;

            return VSConstants.S_OK;
        }

        public int GetGuidEditorType(out Guid pClassID)
        {
            pClassID = GuidList.XinqEditorFactoryGuid;

            return VSConstants.S_OK;
        }

        public int IsDocDataDirty(out int pfDirty)
        {
            pfDirty = _document.IsDirty ? 1 : 0;

            return VSConstants.S_OK;
        }

        public int IsDocDataReadOnly(out int pfReadOnly)
        {
            pfReadOnly = _document.IsReadOnly ? 1 : 0;

            return VSConstants.S_OK;
        }

        public int IsDocDataReloadable(out int pfReloadable)
        {
            pfReloadable = 1;

            return VSConstants.S_OK;
        }

        public int LoadDocData(string pszMkDocument)
        {
            _document = new XinqDocument();
            _document.Load(pszMkDocument);

            _editor.Document = _document;
            _editor.LoadDocument();

            AdviseFileChanges(pszMkDocument);

            return VSConstants.S_OK;
        }

        public int OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
        {
            _documentCookie = docCookie;

            return VSConstants.S_OK;
        }

        public int ReloadDocData(uint grfFlags)
        {
            switch (grfFlags)
            {
                case (uint)_VSRELOADDOCDATA.RDD_IgnoreNextFileChange:
                    // Ignore it. We implemenet IVsFileChangeEvents
                    break;

                case (uint)_VSRELOADDOCDATA.RDD_RemoveUndoStack:
                    _editor.ClearUndoStack();
                    break;
            }

            _document.Reload();
            _editor.LoadDocument();

            return VSConstants.S_OK;
        }

        public int RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            // TODO: Consider grfAttribs?

            UnadviseFileChanges();

            _document.Rename(pszMkDocumentNew);

            AdviseFileChanges(pszMkDocumentNew);

            return VSConstants.S_OK;
        }

        public int SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
        {
            pbstrMkDocumentNew = null;
            pfSaveCanceled = 0;

            UnadviseFileChanges();

            var vsUIShell = GetService(typeof(SVsUIShell)) as IVsUIShell;

            var hr = vsUIShell.SaveDocDataToFile(dwSave, this, null, out pbstrMkDocumentNew, out pfSaveCanceled);

            if (pbstrMkDocumentNew != null)
                AdviseFileChanges(pbstrMkDocumentNew);
            else
                AdviseFileChanges(_document.Filename);

            return hr;
        }

        public int SetDocDataDirty(int fDirty)
        {
            _document.IsDirty = (fDirty == 1);

            return VSConstants.S_OK;
        }

        public int SetDocDataReadOnly(int fReadOnly)
        {
            _document.IsReadOnly = (fReadOnly == 1);

            return VSConstants.S_OK;
        }

        public int SetUntitledDocPath(string pszDocDataPath)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IPersistFileFormat Members

        public int GetClassID(out Guid pClassID)
        {
            pClassID = GuidList.XinqEditorFactoryGuid;

            return VSConstants.S_OK;
        }

        public int GetCurFile(out string ppszFilename, out uint pnFormatIndex)
        {
            ppszFilename = _document.Filename;

            switch (Path.GetExtension(ppszFilename).ToLower())
            {
                case ".xinq":
                    pnFormatIndex = 0;
                    break;

                default:
                    pnFormatIndex = 1;
                    break;
            }

            return VSConstants.S_OK;
        }

        public int GetFormatList(out string ppszFormatList)
        {
            ppszFormatList = "XML Integrated Query Files (*.xinq)\n*.xinq\nAll Files (*.*)\r*.*\n\n";

            return VSConstants.S_OK;
        }

        public int InitNew(uint nFormatIndex)
        {
            return VSConstants.S_OK;
        }

        public int IsDirty(out int pfIsDirty)
        {
            pfIsDirty = _document.IsDirty ? 1 : 0;

            return VSConstants.S_OK;
        }

        public int Load(string pszFilename, uint grfMode, int fReadOnly)
        {
            if (grfMode != 0)
                throw new NotImplementedException();

            var asReadOnly = (fReadOnly == 1);

            _document.Load(pszFilename, asReadOnly);

            return VSConstants.S_OK;
        }

        public int Save(string pszFilename, int fRemember, uint nFormatIndex)
        {
            if (pszFilename == null)
                if (_document.Filename == null)
                    return VSConstants.E_INVALIDARG;

            if (nFormatIndex != 0 && nFormatIndex != 1)
                return VSConstants.E_INVALIDARG;

            var asCopy = (fRemember == 0);

            _document.Save(pszFilename, asCopy);

            return VSConstants.S_OK;
        }

        public int SaveCompleted(string pszFilename)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsFileChangeEvents Members

        public int FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
        {
            if (_ignoreFileChangeLevel > 0)
                return VSConstants.S_OK;

            if (rgpszFile == null)
                return VSConstants.E_INVALIDARG;

            if (rggrfChange == null)
                return VSConstants.E_INVALIDARG;

            for (var i = 0; i < cChanges; i++)
            {
                if (string.Compare(rgpszFile[i], _document.Filename, true, CultureInfo.InvariantCulture) == 0)
                {
                    if ((rggrfChange[i] & (int)_VSFILECHANGEFLAGS.VSFILECHG_Attr) != 0)
                    {
                        _document.DetermineReadOnlyMode();

                        var vsRunningDocumentTable = GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
                        var hr = vsRunningDocumentTable.NotifyDocumentChanged(_documentCookie, 0);
                        ErrorHandler.ThrowOnFailure(hr);
                    }

                    if ((rggrfChange[i] & (int)(_VSFILECHANGEFLAGS.VSFILECHG_Time | _VSFILECHANGEFLAGS.VSFILECHG_Size)) != 0)
                    {
                        // TODO: If VS window is active, setup a timer to prompt user if he/she wants to relaod the
                        // file; othewise, wait until the VS window is activated

                        if (!_document.IsDirty)
                        {
                            _document.Reload();
                            _editor.LoadDocument();
                        }

                        var vsRunningDocumentTable = GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
                        var hr = vsRunningDocumentTable.NotifyDocumentChanged(_documentCookie, (uint)__VSRDTATTRIB.RDTA_DocDataReloaded);
                        ErrorHandler.ThrowOnFailure(hr);
                    }

                    break;
                }
            }

            return VSConstants.S_OK;
        }

        public int DirectoryChanged(string pszDirectory)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsDocDataFileChangeControl Members

        public int IgnoreFileChanges(int fIgnore)
        {
            if (fIgnore != 0)
            {
                _ignoreFileChangeLevel++;
            }
            else
            {
                _ignoreFileChangeLevel--;

                if (_ignoreFileChangeLevel == 0)
                    _document.DetermineReadOnlyMode();
            }

            return VSConstants.S_OK;
        }

        #endregion

        #region IOleCommandTarget Members

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (cCmds != 1)
                return VSConstants.E_INVALIDARG;

            if (prgCmds == null)
                return VSConstants.E_INVALIDARG;

            if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_SUPPORTED;

                switch (prgCmds[0].cmdID)
                {
                    case (uint)VSConstants.VSStd97CmdID.SelectAll:

                        if (_editor.CanSelectAll())
                            prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;

                        return VSConstants.S_OK;

                    case (uint)VSConstants.VSStd97CmdID.Cut:

                        if (_editor.CanCut())
                            prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;

                        return VSConstants.S_OK;

                    case (uint)VSConstants.VSStd97CmdID.Copy:

                        if (_editor.CanCopy())
                            prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;

                        return VSConstants.S_OK;

                    case (uint)VSConstants.VSStd97CmdID.Paste:

                        if (_editor.CanPaste())
                            prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;

                        return VSConstants.S_OK;
                }
            }

            return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
        }

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                switch (nCmdID)
                {
                    case (uint)VSConstants.VSStd97CmdID.SelectAll:

                        _editor.OnSelectAll();
                        return VSConstants.S_OK;

                    case (uint)VSConstants.VSStd97CmdID.Cut:

                        _editor.OnCut();
                        return VSConstants.S_OK;

                    case (uint)VSConstants.VSStd97CmdID.Copy:

                        _editor.OnCopy();
                        return VSConstants.S_OK;

                    case (uint)VSConstants.VSStd97CmdID.Paste:

                        _editor.OnPaste();
                        return VSConstants.S_OK;
                }
            }

            return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
        }

        #endregion

        #region IVsWindowPaneCommit Members

        public int CommitPendingEdit(out int pfCommitFailed)
        {
            pfCommitFailed = 0;

            _editor.CommitPendingEdit();

            return VSConstants.S_OK;
        }

        #endregion
    }
}
