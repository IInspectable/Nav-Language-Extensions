#region Using Directives

using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {
    class DropdownBarClient : SemanticModelServiceDependent, IVsDropdownBarClient {

        // ReSharper disable NotAccessedField.Local
        readonly IWpfTextView _textView;
        readonly IVsDropdownBarManager _manager;
        readonly IVsCodeWindow _codeWindow;
        readonly IntPtr _imageList;
        readonly IVsImageService2 _imageService;
        // ReSharper restore NotAccessedField.Local
        IVsDropdownBar _dropdownBar;

        public DropdownBarClient(IWpfTextView textView,
            IVsDropdownBarManager manager,
            IVsCodeWindow codeWindow,
            IServiceProvider serviceProvider): base(textView.TextBuffer) {

            _textView     = textView;
            _manager      = manager;
            _codeWindow   = codeWindow;
            _imageService = (IVsImageService2)serviceProvider.GetService(typeof(SVsImageService));
            _imageList    = GetImageList(serviceProvider);
        }

        int IVsDropdownBarClient.SetDropdownBar(IVsDropdownBar pDropdownBar) {

            _dropdownBar = pDropdownBar;

            UpdateDropDownEntries();

            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetComboAttributes(int iCombo, out uint pcEntries, out uint puEntryType, out IntPtr phImageList) {

            // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
            puEntryType = (uint)(DROPDOWNENTRYTYPE.ENTRY_TEXT | DROPDOWNENTRYTYPE.ENTRY_ATTR | DROPDOWNENTRYTYPE.ENTRY_IMAGE);
            // ReSharper restore BitwiseOperatorOnEnumWithoutFlags
            phImageList = _imageList;
            pcEntries   = 1;

            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetEntryText(int iCombo, int iIndex, out string ppszText) {

            ppszText = null;

            switch (iCombo) {
                case 0:
                    ppszText = SemanticModelService?.SemanticModelResult?.CodeGenerationUnit.Syntax.SyntaxTree.FileInfo?.Name?? "";
                    break;
                case 1:
                    ppszText = "OffenePosten";
                    break;
                case 2:
                    ppszText = "OnSucheClick";
                    break;
            }
           
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetEntryAttributes(int iCombo, int iIndex, out uint pAttr) {

            DROPDOWNFONTATTR attributes = DROPDOWNFONTATTR.FONTATTR_PLAIN;

            // attributes |= DROPDOWNFONTATTR.FONTATTR_GRAY;
            // attributes |= DROPDOWNFONTATTR.FONTATTR_BOLD;
            pAttr = (uint)attributes;
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetEntryImage(int iCombo, int iIndex, out int piImageIndex) {
            piImageIndex = 0;
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.OnItemSelected(int iCombo, int iIndex) {
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.OnItemChosen(int iCombo, int iIndex) {
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.OnComboGetFocus(int iCombo) {
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetComboTipText(int iCombo, out string pbstrText) {
            
            pbstrText = "Tip Text";
            return VSConstants.S_OK;
        }

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
            UpdateDropDownEntries();
        }

        void UpdateDropDownEntries() {
            // TODO UpdateDropDownEntries
            _dropdownBar?.RefreshCombo(0, 0);
        }

        static IntPtr GetImageList(IServiceProvider serviceProvider) {

            var vsShell = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
            if (vsShell != null) {
                object varImageList;
                int hresult = vsShell.GetProperty((int)__VSSPROPID.VSSPROPID_ObjectMgrTypesImgList, out varImageList);
                if (ErrorHandler.Succeeded(hresult) && varImageList != null) {
                    return (IntPtr)(int)varImageList;
                }
            }

            return IntPtr.Zero;
        }
    }
}
