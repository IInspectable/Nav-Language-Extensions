#region Using Directives

using System;
using System.Linq;
using System.Collections.Immutable;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {
  
    class DropdownBarClient : SemanticModelServiceDependent, IVsDropdownBarClient, IDisposable {

        // ReSharper disable NotAccessedField.Local
        readonly IWpfTextView _textView;
        readonly IVsDropdownBarManager _manager;
        readonly IVsCodeWindow _codeWindow;
        readonly IntPtr _imageList;
        readonly IVsImageService2 _imageService;
        // ReSharper restore NotAccessedField.Local
        IVsDropdownBar _dropdownBar;

        ImmutableList<NavigationItem> _projectItems;
        ImmutableList<NavigationItem> _taskItems;
        ImmutableList<NavigationItem> _memberItems;

        public DropdownBarClient(
            IWpfTextView textView,
            IVsDropdownBarManager manager,
            IVsCodeWindow codeWindow,
            
            IServiceProvider serviceProvider): base(textView.TextBuffer) {

            _textView     = textView;
            _manager      = manager;
            _codeWindow   = codeWindow;
            _imageService = (IVsImageService2)serviceProvider.GetService(typeof(SVsImageService));
            _imageList    = GetImageList(serviceProvider);

            _projectItems = ImmutableList<NavigationItem>.Empty;
            _taskItems    = ImmutableList<NavigationItem>.Empty;
            _memberItems  = ImmutableList<NavigationItem>.Empty;
            
            _textView.Caret.PositionChanged += OnCaretPositionChanged;
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
            pcEntries   = (uint)GetItems(iCombo).Count;
            
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetEntryText(int iCombo, int iIndex, out string ppszText) {

            var items = GetItems(iCombo);
            ppszText = iIndex >= items.Count ? "" : items[iIndex].DisplayName;
           
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetEntryAttributes(int iCombo, int iIndex, out uint pAttr) {

            DROPDOWNFONTATTR attributes = DROPDOWNFONTATTR.FONTATTR_PLAIN;

            var entries       = GetItems(iCombo);
            var selectedIndex = GetActiveSelection(iCombo);
            var caretPosition = _textView.Caret.Position.BufferPosition.Position;

            if(entries.Any() && iIndex < entries.Count &&
                iIndex == selectedIndex &&
                (caretPosition < entries[selectedIndex].Start ||
                 caretPosition > entries[selectedIndex].End)) {

                attributes = DROPDOWNFONTATTR.FONTATTR_GRAY;
            }

            pAttr = (uint)attributes;

            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetEntryImage(int iCombo, int iIndex, out int piImageIndex) {

            var items = GetItems(iCombo);
            piImageIndex = iIndex >= items.Count ? 0 : items[iIndex].ImageIndex;

            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.OnItemSelected(int iCombo, int iIndex) {
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.OnItemChosen(int iCombo, int iIndex) {

            if(_dropdownBar == null) {
                return VSConstants.E_UNEXPECTED;
            }

            var items = GetItems(iCombo);
            if(items != null && iIndex < items.Count) {
                
                var item = items[iIndex];
                if (item.NavigationPoint < 0) {
                    return VSConstants.S_OK;
                }

                _dropdownBar.RefreshCombo(iCombo, iIndex);

                NavLanguagePackage. NavigateToLocation(_textView, item.NavigationPoint);                
            }

            return VSConstants.S_OK;
        }
        
        int IVsDropdownBarClient.OnComboGetFocus(int iCombo) {
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetComboTipText(int iCombo, out string pbstrText) {
            
            pbstrText = "Use the dropdown to view and navigate to other items in the file.";
            return VSConstants.S_OK;
        }

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
            UpdateDropDownEntries();
        }

        public override void Dispose() {
            base.Dispose();
            _textView.Caret.PositionChanged -= OnCaretPositionChanged;
        }

        void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {

            // TODO Im Hintergrund durchführen?s
            SetActiveSelection(TaskComboIndex);
            SetActiveSelection(MemberComboIndex);            
        }
        
        ImmutableList<NavigationItem> GetItems(int iCombo) {
            switch (iCombo) {
                case ProjectComboIndex:
                    return _projectItems;
                case TaskComboIndex:
                    return _taskItems;
                case MemberComboIndex:
                    return _memberItems;
                default:
                    return ImmutableList<NavigationItem>.Empty;
            }
        }

        void UpdateDropDownEntries() {

            UpdateProjectItems();
            UpdateTaskItems();
            UpdateMemberItems();
        }

        const int ProjectComboIndex = 0;
        const int TaskComboIndex    = 1;
        const int MemberComboIndex  = 2;

        void UpdateProjectItems() {

            _projectItems= ImmutableList<NavigationItem>.Empty;
            var cgu = SemanticModelService?.SemanticModelResult?.CodeGenerationUnit;
            if (cgu != null) {
                _projectItems =
                    new[] {
                        new NavigationItem(
                            displayName    : SemanticModelService?.SemanticModelResult?.Snapshot.TextBuffer.GetContainingProject()?.Name,
                            imageIndex     : 0, 
                            location       : null,
                            navigationPoint: -1)
                    }.ToImmutableList();
            }
            _dropdownBar?.RefreshCombo(ProjectComboIndex, 0);
        }

        void UpdateTaskItems() {

            _taskItems = ImmutableList<NavigationItem>.Empty;
            var cgu = SemanticModelService?.SemanticModelResult?.CodeGenerationUnit;
            if(cgu != null) {
                _taskItems = TaskNavigationItemBuilder.Build(cgu);
            }

            SetActiveSelection(TaskComboIndex);
        }

        void UpdateMemberItems() {

            // TODO Member nur für aktuellen Task..
            _memberItems = ImmutableList<NavigationItem>.Empty;

            var cgu = SemanticModelService?.SemanticModelResult?.CodeGenerationUnit;
            if(cgu != null) {
                _memberItems = MemberNavigationItemBuilder.Build(cgu);
            }

            SetActiveSelection(MemberComboIndex);
        }

        void SetActiveSelection(int comboBoxId) {
            if (_dropdownBar == null) {
                return;
            }

            var newIndex = -1;

            var entries = GetItems(comboBoxId);

            if(entries.Any()) {

                var newPosition    = _textView.Caret.Position.BufferPosition.Position;
                var entriesByScope = entries.OrderBy(entry => entry.End);
                var activeEntry    = entriesByScope.FirstOrDefault(entry => newPosition >= entry.Start && newPosition <= entry.End);

                if(activeEntry != null) {
                    newIndex = entries.IndexOf(activeEntry);
                } else {
                    // If outside all entries, select the entry just before it
                    var closestEntry = entriesByScope.LastOrDefault(entry => newPosition >= entry.End);
                    if(closestEntry == null) {
                        // if the mouse is before any entries, select the first one
                        closestEntry = entries.OrderBy(entry => entry.Start).First();
                    }

                    newIndex = entries.IndexOf(closestEntry);
                }
            }

            _dropdownBar.RefreshCombo(comboBoxId, newIndex);
        }

        int GetActiveSelection(int iCombo) {

            if (_dropdownBar == null) {
                return -1;
            }
            int sel = -1;
             _dropdownBar?.GetCurrentSelection(iCombo, out sel);
            return sel;
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
