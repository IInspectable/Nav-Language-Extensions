#region Using Directives

using System;
using System.Collections.Immutable;
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
        ImmutableList<NavigationItem> _taskItems;
        ImmutableList<NavigationItem> _memberItems;

        public DropdownBarClient(IWpfTextView textView,
            IVsDropdownBarManager manager,
            IVsCodeWindow codeWindow,
            IServiceProvider serviceProvider): base(textView.TextBuffer) {

            _textView     = textView;
            _manager      = manager;
            _codeWindow   = codeWindow;
            _imageService = (IVsImageService2)serviceProvider.GetService(typeof(SVsImageService));
            _imageList    = GetImageList(serviceProvider);
            _taskItems    = ImmutableList<NavigationItem>.Empty;
            _memberItems  = ImmutableList<NavigationItem>.Empty;
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

            if (iCombo == 1) {
                pcEntries= (uint)_taskItems.Count;
                return VSConstants.S_OK;
            }
            if(iCombo == 2) {
                pcEntries = (uint)_memberItems.Count;
                return VSConstants.S_OK;
            }

            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetEntryText(int iCombo, int iIndex, out string ppszText) {

            ppszText = null;

            switch (iCombo) {
                case 0:
                    ppszText = SemanticModelService?.SemanticModelResult?.CodeGenerationUnit.Syntax.SyntaxTree.FileInfo?.Name?? "";
                    break;
                case 1:
                    ppszText = iIndex >= _taskItems.Count ? "" : _taskItems[iIndex].DisplayName;
                    break;
                case 2:
                    ppszText = iIndex >= _memberItems.Count ? "" : _memberItems[iIndex].DisplayName;
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
            
            _taskItems   = ImmutableList<NavigationItem>.Empty;
            _memberItems = ImmutableList<NavigationItem>.Empty;

            var cgu = SemanticModelService?.SemanticModelResult?.CodeGenerationUnit;
            if (cgu != null) {
                _taskItems   = TaskNavigationItemBuilder.Build(cgu);
                _memberItems = MemberNavigationItemBuilder.Build(cgu);
            }

            _dropdownBar?.RefreshCombo(0, 0);
            _dropdownBar?.RefreshCombo(1, 0);
            _dropdownBar?.RefreshCombo(2, 0);
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
