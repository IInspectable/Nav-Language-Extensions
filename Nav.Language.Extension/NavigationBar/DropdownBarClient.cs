﻿#region Using Directives

using System;
using System.Linq;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Windows.Forms;
using JetBrains.Annotations;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

using Pharmatechnik.Nav.Utilities.Logging;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

using Control = System.Windows.Controls.Control;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {
  
    class DropdownBarClient : SemanticModelServiceDependent, IVsDropdownBarClient, IVsCodeWindowEvents, IDisposable {

        static readonly Logger Logger = Logger.Create<DropdownBarClient>();

        readonly IVsCodeWindow _codeWindow;
        readonly IVsDropdownBarManager _manager;
        readonly IntPtr _imageListHandle;
        readonly WorkspaceRegistration _workspaceRegistration;
        readonly Dictionary<int, int> _activeSelections;
        readonly Dispatcher _dispatcher;
        readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;
        readonly Dictionary<IVsTextView, IWpfTextView> _trackedViews;
        readonly IDisposable _comEventSink;

        [CanBeNull]
        Workspace _workspace;
        IVsDropdownBar _dropdownBar;
        int _focusedCombo;

        ImmutableList<NavigationItem> _projectItems;
        ImmutableList<NavigationItem> _taskItems;


        public DropdownBarClient(
            ITextBuffer textBuffer,
            IVsDropdownBarManager manager,
            IVsCodeWindow codeWindow,
            
            IServiceProvider serviceProvider): base(textBuffer) {

            Logger.Trace($"{nameof(DropdownBarClient)}:Ctor");


            var imageService = (IVsImageService2)serviceProvider.GetService(typeof(SVsImageService));

            var comboBoxBackgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxBackgroundColorKey);

            _manager          = manager;
            _codeWindow       = codeWindow;
            _imageListHandle  = NavigationBarImages.GetImageList(comboBoxBackgroundColor, imageService);
            _projectItems     = ImmutableList<NavigationItem>.Empty;
            _taskItems        = ImmutableList<NavigationItem>.Empty;
            _dispatcher       = Dispatcher.CurrentDispatcher;
            _activeSelections = new Dictionary<int, int>();
            _focusedCombo     = -1;
            _trackedViews     = new Dictionary<IVsTextView, IWpfTextView>();

            _workspaceRegistration = Workspace.GetWorkspaceRegistration(TextBuffer.AsTextContainer());
            _workspaceRegistration.WorkspaceChanged += OnWorkspaceRegistrationChanged;

            var componentModel = (IComponentModel)serviceProvider.GetService(typeof(SComponentModel));
            _editorAdaptersFactoryService=componentModel.GetService<IVsEditorAdaptersFactoryService>();

            _comEventSink = ComEventSink.Advise<IVsCodeWindowEvents>(codeWindow, this);

            IVsTextView pTextView;
            codeWindow.GetPrimaryView(out pTextView);            
            ConnectView(pTextView);

            codeWindow.GetSecondaryView(out pTextView);
            ConnectView(pTextView);

            ConnectToWorkspace(_workspaceRegistration.Workspace);           
        }

        void ConnectView(IVsTextView vsTextView) {

            if(vsTextView == null || _trackedViews.ContainsKey(vsTextView)) {
                return;

            }

            var wpfTextView = _editorAdaptersFactoryService.GetWpfTextView(vsTextView);
            if(wpfTextView == null) {
                return;
            }

            wpfTextView.Caret.PositionChanged += OnCaretPositionChanged;
            wpfTextView.GotAggregateFocus     += OnTextViewGotAggregateFocus;

            _trackedViews.Add(vsTextView, wpfTextView);
        }

        void DisconnectView(IVsTextView vsTextView) {

            if (vsTextView == null || !_trackedViews.ContainsKey(vsTextView)) {
                return;
            }

            var wpfTextView = _trackedViews[vsTextView];

            wpfTextView.Caret.PositionChanged -= OnCaretPositionChanged;
            wpfTextView.GotAggregateFocus     -= OnTextViewGotAggregateFocus;

            _trackedViews.Remove(vsTextView);
        }

        public override void Dispose() {

            Logger.Trace($"{nameof(DropdownBarClient)}:{nameof(Dispose)}");

            base.Dispose();

            _workspaceRegistration.WorkspaceChanged -= OnWorkspaceRegistrationChanged;

            foreach(var view in _trackedViews.Keys.ToList()) {
                DisconnectView(view);
            }

            _manager?.RemoveDropdownBar();
            _comEventSink?.Dispose();

            DisconnectFromWorkspace();
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

            if (_workspace == null) {
                return;
            }

            _workspace.WorkspaceChanged -= OnWorkspaceChanged;
            _workspace = null;
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

            UpdateNavigationItems();

            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetComboAttributes(int iCombo, out uint pcEntries, out uint puEntryType, out IntPtr phImageList) {

            // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
            puEntryType = (uint)(DROPDOWNENTRYTYPE.ENTRY_TEXT | DROPDOWNENTRYTYPE.ENTRY_ATTR | DROPDOWNENTRYTYPE.ENTRY_IMAGE);
            // ReSharper restore BitwiseOperatorOnEnumWithoutFlags
            phImageList = _imageListHandle;
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
            var selectedIndex = GetActiveSelection(iCombo);
            var caretPosition = GetCurrentView().Caret.Position.BufferPosition.Position;

            if(_focusedCombo!=iCombo &&
                entries.Any() && iIndex < entries.Count &&
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
            #if ShowMemberCombobox
            if(iCombo == TaskComboIndex) {            
                SetActiveSelection(MemberComboIndex);    
            }
            #endif
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.OnItemChosen(int iCombo, int iIndex) {

            if(_dropdownBar == null) {
                return VSConstants.E_UNEXPECTED;
            }

            var item = GetActiveSelectionItem(iCombo, iIndex);

            if(item?.NavigationPoint >= 0) {

                _dropdownBar.RefreshCombo(iCombo, iIndex);

                NavLanguagePackage.NavigateToLocation(GetCurrentView(), item.NavigationPoint);
            } else {
                // ReSharper disable once SuspiciousTypeConversion.Global
                (GetCurrentView() as Control)?.Focus();
            }

            return VSConstants.S_OK;
        }
        
        int IVsDropdownBarClient.OnComboGetFocus(int iCombo) {
            _focusedCombo = iCombo;
            SetActiveSelection(TaskComboIndex);
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetComboTipText(int iCombo, out string pbstrText) {

            pbstrText = GetActiveSelectionItem(iCombo)?.DisplayName ?? "";
            if(pbstrText != "") {
                pbstrText += Environment.NewLine + Environment.NewLine;
            }
            pbstrText += "Use the dropdown to view and navigate to other items in the file.";

            return VSConstants.S_OK;
        }

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
            _dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(UpdateNavigationItems));
        }
        
        void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
            using (Logger.LogBlock(nameof(OnCaretPositionChanged))) {
                _dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => SetActiveSelection(TaskComboIndex)));
                #if ShowMemberCombobox
                _dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => SetActiveSelection(MemberComboIndex)));
                #endif
            }
        }

        void OnTextViewGotAggregateFocus(object sender, EventArgs e) {

            // Es kann keine Combobox mehr einen Fokus haben
            _focusedCombo = -1;
            // Leider bekommen wir ein Project Reload nicht mit. Hier nochmal die Chance das aktuelle Projekt zu aktualisieren
            UpdateProjectItems();

            // Selektion aktualisieren, um FONTATTR_GRAY entprechend zu setzen 
            SetActiveSelection(TaskComboIndex);
        }

        ImmutableList<NavigationItem> GetItems(int iCombo) {
            switch (iCombo) {
                case ProjectComboIndex:
                    return _projectItems;
                case TaskComboIndex:
                    return _taskItems;
                case MemberComboIndex:
                    var taskItem= GetActiveSelectionItem(TaskComboIndex);
                    return taskItem?.Children?? ImmutableList<NavigationItem>.Empty;
                default:
                    return ImmutableList<NavigationItem>.Empty;
            }
        }

        void UpdateNavigationItems() {
            using (Logger.LogBlock(nameof(UpdateNavigationItems))) {

                UpdateProjectItems();
                UpdateTaskItems();
                #if ShowMemberCombobox
                UpdateMemberItems();
                #endif
            }
        }

        const int ProjectComboIndex = 0;
        const int TaskComboIndex    = 1;
        const int MemberComboIndex  = 2;

        void UpdateProjectItems() {

            _projectItems = ProjectItemBuilder.Build(SemanticModelService?.SemanticModelResult);
           
            _dropdownBar?.RefreshCombo(ProjectComboIndex, 0);
        }

        void UpdateTaskItems() {

            _taskItems = TaskNavigationItemBuilder.Build(SemanticModelService?.SemanticModelResult);

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
            // Wir speichern die Selektion hier, weil u.a. während des Aufrufs von GetEntryAttributes 
            // _dropdownBar.GetCurrentSelection nicht den aktuellsten Stand wiederspiegelt.
            _activeSelections[comboBoxId] = newIndex;
            // Hier reicht kein _dropdownBar.SetCurrentSelection, da wir u.U. auch die Font Attribute ändern müssen (ausgegraut/nicht ausgegraut)            
            _dropdownBar.RefreshCombo(comboBoxId, newIndex);
        }

        int GetActiveSelection(int comboBoxId) {

            int selection;
            if (_activeSelections.TryGetValue(comboBoxId, out selection)) {
                return selection;
            }
            return -1;
        }

        [CanBeNull]
        NavigationItem GetActiveSelectionItem(int iCombo) {

            var index = GetActiveSelection(iCombo);
            if(index < 0) {
                return null;
            }

            return GetActiveSelectionItem(iCombo, index);
        }

        [CanBeNull]
        NavigationItem GetActiveSelectionItem(int iCombo, int iIndex) {

            if(iIndex < 0) {
                return null;
            }

            var items = GetItems(iCombo);
            return iIndex < items.Count ? items[iIndex] : null;
        }

        /// <summary>
        /// Berechnet die zu wählende Selektion für die angegebene Combobox ausgehend von der aktuellen Caretposition
        /// </summary>
        int CalculateActiveSelection(int comboBoxId) {

            var newIndex = -1;
            var items    = GetItems(comboBoxId);

            if(items.Any()) {

                var caretPosition = GetCurrentView().Caret.Position.BufferPosition.Position;
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
        
        IWpfTextView GetCurrentView() {
            IVsTextView lastActiveView;
            _codeWindow.GetLastActiveView(out lastActiveView);
            lastActiveView = lastActiveView ?? _trackedViews.Keys.FirstOrDefault();
            return _editorAdaptersFactoryService.GetWpfTextView(lastActiveView);
        }

        int IVsCodeWindowEvents.OnNewView(IVsTextView pView) {
            ConnectView(pView);
            return VSConstants.S_OK;
        }

        int IVsCodeWindowEvents.OnCloseView(IVsTextView pView) {
            DisconnectView(pView);
            return VSConstants.S_OK;
        }
    }
}