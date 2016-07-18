//#define ShowMemberCombobox
#region Using Directives

using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Immutable;

using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

using Pharmatechnik.Nav.Language.Extension.LanguageService;
using Pharmatechnik.Nav.Utilities.Logging;
using Control = System.Windows.Controls.Control;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {
  
    class DropdownBarClient : SemanticModelServiceDependent, IVsDropdownBarClient, IDisposable {

        static readonly Logger Logger = Logger.Create<DropdownBarClient>();

        // ReSharper disable NotAccessedField.Local
        readonly IVsCodeWindow _codeWindow;
        readonly IVsImageService2 _imageService;
        readonly IVsDropdownBarManager _manager;
        // ReSharper restore NotAccessedField.Local
        readonly IWpfTextView _textView;
        readonly ImageList _imageList;
        readonly WorkspaceRegistration _workspaceRegistration;
        [CanBeNull]
        Workspace _workspace;
        IVsDropdownBar _dropdownBar;

        ImmutableList<NavigationItem> _projectItems;
        ImmutableList<NavigationItem> _taskItems;
        
        public DropdownBarClient(
            IWpfTextView textView,
            IVsDropdownBarManager manager,
            IVsCodeWindow codeWindow,
            
            IServiceProvider serviceProvider): base(textView.TextBuffer) {

            Logger.Trace($"{nameof(DropdownBarClient)}:Ctor");

            var comboBoxBackgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxBackgroundColorKey);

            _textView     = textView;
            _manager      = manager;
            _codeWindow   = codeWindow;
            _imageService = (IVsImageService2)serviceProvider.GetService(typeof(SVsImageService));
            _imageList    = NavigationImages.CreateImageList(comboBoxBackgroundColor);
            _projectItems = ImmutableList<NavigationItem>.Empty;
            _taskItems    = ImmutableList<NavigationItem>.Empty;

            _textView.Caret.PositionChanged += OnCaretPositionChanged;
            _workspaceRegistration = Workspace.GetWorkspaceRegistration(TextBuffer.AsTextContainer());
            _workspaceRegistration.WorkspaceChanged += OnWorkspaceRegistrationChanged;

            if (_workspaceRegistration.Workspace != null) {
                ConnectToWorkspace(_workspaceRegistration.Workspace);
            }
        }

        #region Workspace Management

        void OnWorkspaceRegistrationChanged(object sender, EventArgs e) {

            DisconnectFromWorkspace();

            var newWorkspace = _workspaceRegistration.Workspace;

            ConnectToWorkspace(newWorkspace);
        }

        void ConnectToWorkspace([CanBeNull] Workspace workspace) {

            DisconnectFromWorkspace();

            _workspace = workspace;

            if (_workspace != null) {
                _workspace.WorkspaceChanged += OnWorkspaceChanged;
            }

            UpdateProjectItems();
        }

        void DisconnectFromWorkspace() {
            
            if (_workspace != null) {
                _workspace.WorkspaceChanged -= OnWorkspaceChanged;
                _workspace = null;
            }
        }

        void OnWorkspaceChanged(object sender, WorkspaceChangeEventArgs args) {

            // We're getting an event for a workspace we already disconnected from
            if (args.NewSolution.Workspace != _workspace) {
                return;
            }

            if (args.Kind == WorkspaceChangeKind.SolutionChanged  ||
                args.Kind == WorkspaceChangeKind.SolutionAdded    ||
                args.Kind == WorkspaceChangeKind.SolutionRemoved  ||
                args.Kind == WorkspaceChangeKind.SolutionCleared  ||
                args.Kind == WorkspaceChangeKind.SolutionReloaded ||
                args.Kind == WorkspaceChangeKind.ProjectAdded     || 
                args.Kind == WorkspaceChangeKind.ProjectChanged   ||
                args.Kind == WorkspaceChangeKind.ProjectReloaded  ||
                args.Kind == WorkspaceChangeKind.ProjectRemoved) {
                UpdateProjectItems();
            }
        }

        #endregion

        int IVsDropdownBarClient.SetDropdownBar(IVsDropdownBar pDropdownBar) {

            _dropdownBar = pDropdownBar;

            UpdateDropDownEntries();

            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetComboAttributes(int iCombo, out uint pcEntries, out uint puEntryType, out IntPtr phImageList) {

            // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
            puEntryType = (uint)(DROPDOWNENTRYTYPE.ENTRY_TEXT | DROPDOWNENTRYTYPE.ENTRY_ATTR | DROPDOWNENTRYTYPE.ENTRY_IMAGE);
            // ReSharper restore BitwiseOperatorOnEnumWithoutFlags
            phImageList = _imageList.Handle;
            pcEntries   = (uint) GetItems(iCombo).Count;
            
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
            var selectedIndex = CalculateActiveSelection(iCombo);
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
            if(iCombo == TaskComboIndex) {
                SetActiveSelection(MemberComboIndex);
            }
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.OnItemChosen(int iCombo, int iIndex) {

            if(_dropdownBar == null) {
                return VSConstants.E_UNEXPECTED;
            }

            var item = GetCurrentSelectionItem(iCombo, iIndex);

            if(item?.NavigationPoint >= 0) {

                _dropdownBar.RefreshCombo(iCombo, iIndex);

                NavLanguagePackage.NavigateToLocation(_textView, item.NavigationPoint);
            } else {
                // ReSharper disable once SuspiciousTypeConversion.Global
                (_textView as Control)?.Focus();
            }

            return VSConstants.S_OK;
        }
        
        int IVsDropdownBarClient.OnComboGetFocus(int iCombo) {
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetComboTipText(int iCombo, out string pbstrText) {

            pbstrText = GetCurrentSelectionItem(iCombo)?.DisplayName ?? "";
            if(pbstrText != "") {
                pbstrText += Environment.NewLine + Environment.NewLine;
            }
            pbstrText += "Use the dropdown to view and navigate to other items in the file.";

            return VSConstants.S_OK;
        }

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
            UpdateDropDownEntries();
        }

        public override void Dispose() {

            Logger.Trace($"{nameof(DropdownBarClient)}:{nameof(Dispose)}");

            base.Dispose();
            _textView.Caret.PositionChanged -= OnCaretPositionChanged;
            _imageList.Dispose();

            _workspaceRegistration.WorkspaceChanged -= OnWorkspaceRegistrationChanged;
            DisconnectFromWorkspace();
        }

        void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {

            // TODO Im Hintergrund durchführen?
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
                    var taskItem= GetCurrentSelectionItem(TaskComboIndex);
                    return taskItem?.Children?? ImmutableList<NavigationItem>.Empty;
                default:
                    return ImmutableList<NavigationItem>.Empty;
            }
        }

        void UpdateDropDownEntries() {

            UpdateProjectItems();
            UpdateTaskItems();
#if ShowMemberCombobox
            UpdateMemberItems();
#endif
        }

        const int ProjectComboIndex = 0;
        const int TaskComboIndex    = 1;
        const int MemberComboIndex  = 2;

        void UpdateProjectItems() {

            _projectItems = ProjectItemBuilder.Build(SemanticModelService?.SemanticModelResult);
           
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

#if ShowMemberCombobox
        void UpdateMemberItems() {
            
            SetActiveSelection(MemberComboIndex);
        }
#endif

        void SetActiveSelection(int comboBoxId) {

            if (_dropdownBar == null) {
                return;
            }

            var newIndex = CalculateActiveSelection(comboBoxId);

            _dropdownBar.RefreshCombo(comboBoxId, newIndex);
        }

        int CalculateActiveSelection(int comboBoxId) {

            var newIndex = -1;
            var items    = GetItems(comboBoxId);

            if(items.Any()) {

                var caretPosition = _textView.Caret.Position.BufferPosition.Position;
                var activeItem = items.FirstOrDefault(entry => caretPosition >= entry.Start && caretPosition <= entry.End);

                if(activeItem != null) {
                    newIndex = items.IndexOf(activeItem);
                } else {
                    // Den ersten Eintrag nach dem Cursor wählen
                    var closestEntry = items.FirstOrDefault(entry => caretPosition < entry.Start && caretPosition < entry.End);
                    if(closestEntry == null) {
                        // Den letzten Eintrag wählen
                        closestEntry = items.Last();
                    }

                    newIndex = items.IndexOf(closestEntry);
                }
            }
            return newIndex;
        }

        int GetCurrentSelectionIndex(int iCombo) {

            if (_dropdownBar == null) {
                return -1;
            }
            int sel = -1;
             _dropdownBar?.GetCurrentSelection(iCombo, out sel);
            return sel;
        }

        [CanBeNull]
        NavigationItem GetCurrentSelectionItem(int iCombo) {

            var index = GetCurrentSelectionIndex(iCombo);
            if(index < 0) {
                return null;
            }

            return GetCurrentSelectionItem(iCombo, index);
        }

        [CanBeNull]
        NavigationItem GetCurrentSelectionItem(int iCombo, int iIndex) {

            if (iIndex < 0) {
                return null;
            }

            var items = GetItems(iCombo);
            return iIndex < items.Count ? items[iIndex] : null;
        }        
    }
}
