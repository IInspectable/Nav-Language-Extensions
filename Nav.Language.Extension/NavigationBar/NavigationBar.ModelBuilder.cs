#region Using Directives

using System;
using System.Collections.Immutable;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    partial class NavigationBar: TypeAndMemberDropdownBars {

        sealed class ModelBuilder: SemanticModelServiceDependent {

            readonly NavigationBar         _parent;
            readonly WorkspaceRegistration _workspaceRegistration;

            [CanBeNull]
            Workspace _workspace;

            [NotNull]
            ImmutableList<NavigationBarItem> _projectItems;

            [NotNull]
            ImmutableList<NavigationBarItem> _taskItems;

            public ModelBuilder(NavigationBar parent, ITextBuffer textBuffer): base(textBuffer) {

                _parent                = parent;
                _workspaceRegistration = Workspace.GetWorkspaceRegistration(textBuffer.AsTextContainer());

                _workspaceRegistration.WorkspaceChanged += OnWorkspaceRegistrationChanged;

                _projectItems = ImmutableList<NavigationBarItem>.Empty;
                _taskItems    = ImmutableList<NavigationBarItem>.Empty;

                UpdateItems(notifyModelChanged: false);
                ConnectToWorkspace(_workspaceRegistration.Workspace, notifyModelChanged: false);
            }

            public override void Dispose() {
                base.Dispose();
                DisconnectFromWorkspace();
            }

            public ImmutableList<NavigationBarItem> ProjectItems => _projectItems;
            public ImmutableList<NavigationBarItem> TaskItems    => _taskItems;

            protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
                UpdateItems();
            }

            void UpdateItems(bool notifyModelChanged = true) {

                using (Logger.LogBlock(nameof(UpdateItems))) {

                    _projectItems = NavigationBarProjectItemBuilder.Build(SemanticModelService?.CodeGenerationUnitAndSnapshot);
                    _taskItems    = NavigationBarTaskItemBuilder.Build(SemanticModelService?.CodeGenerationUnitAndSnapshot);

                    if (notifyModelChanged) {
                        NotifyModelChanged();
                    }
                }
            }

            void NotifyModelChanged() {
                _parent.OnModelChanged();
            }

            #region Workspace Management

            void OnWorkspaceRegistrationChanged(object sender, EventArgs e) {

                DisconnectFromWorkspace();

                var newWorkspace = _workspaceRegistration.Workspace;

                ConnectToWorkspace(newWorkspace);
            }

            void ConnectToWorkspace([CanBeNull] Workspace workspace, bool notifyModelChanged = true) {

                DisconnectFromWorkspace();

                _workspace = workspace;

                if (_workspace != null) {
                    _workspace.WorkspaceChanged += OnWorkspaceChanged;
                }

                if (notifyModelChanged) {
                    NotifyModelChanged();
                }
            }

            void DisconnectFromWorkspace() {

                if (_workspace == null) {
                    return;
                }

                _workspace.WorkspaceChanged -= OnWorkspaceChanged;
                _workspace                  =  null;
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

                    UpdateItems();
                }
            }

            #endregion

        }

    }

}