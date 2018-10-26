﻿#region Using Directives

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell.FindAllReferences;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.Extension.Utilities;
using Pharmatechnik.Nav.Language.FindReferences;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class FindReferencesContext: IFindReferencesContext, ITableDataSource, ITableEntriesSnapshotFactory {

        readonly object _gate = new object();

        readonly CancellationTokenSource  _cancellationTokenSource;
        readonly IFindAllReferencesWindow _findReferencesWindow;
        readonly ProjectMapper            _projectMapper;

        TableEntriesSnapshot _lastSnapshot;
        ITableDataSink       _tableDataSink;

        ImmutableArray<DefinitionEntry> _definitionEntries = ImmutableArray<DefinitionEntry>.Empty;
        ImmutableArray<Entry>           _entries           = ImmutableArray<Entry>.Empty;

        public FindReferencesContext(FindReferencesPresenter presenter, IFindAllReferencesWindow findReferencesWindow, ProjectMapper projectMapper) {

            Presenter = presenter;

            _cancellationTokenSource = new CancellationTokenSource();

            _findReferencesWindow = findReferencesWindow;
            _projectMapper        = projectMapper;

            TableControl = (IWpfTableControl2) findReferencesWindow.TableControl;
            //TableControl.GroupingsChanged += OnTableControlGroupingsChanged;

            _findReferencesWindow.Closed += OnFindReferencesWindowClosed;

            _findReferencesWindow.Manager.AddSource(this);

        }

        public FindReferencesPresenter Presenter { get; }
        public string                  Message   { get; private set; }

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public Task ReportMessageAsync(string message) {
            Message = message;
            return Task.CompletedTask;
        }

        public Task OnDefinitionFoundAsync(DefinitionItem definitionItem) {

            EnsureDefinitionEntry(definitionItem);

            return Task.CompletedTask;
        }

        DefinitionEntry EnsureDefinitionEntry(DefinitionItem definitionItem, ImageMoniker? imageMoniker = null) {

            lock (_gate) {

                var definitionEntry = _definitionEntries.FirstOrDefault(de => de.DefinitionItem == definitionItem);

                if (definitionEntry == null) {
                    definitionEntry    = DefinitionEntry.Create(Presenter, definitionItem, imageMoniker);
                    _definitionEntries = _definitionEntries.Add(definitionEntry);
                }

                return definitionEntry;
            }
        }

        public Task OnReferenceFoundAsync(ReferenceItem referenceItem) {

            var projectInfo     = _projectMapper.GetProjectInfo(referenceItem.Location.FilePath);
            var definitionEntry = EnsureDefinitionEntry(referenceItem.Definition);
            var referenceEntry  = ReferenceEntry.Create(Presenter, definitionEntry, referenceItem, projectInfo);

            return OnEntryFoundAsync(referenceEntry);

        }

        Task OnEntryFoundAsync(Entry entry) {
            lock (_gate) {
                _entries = _entries.Add(entry);
                CurrentVersionNumber++;
            }

            NotifyChange();
            return Task.CompletedTask;
        }

        public async Task OnCompletedAsync() {

            await CreateMissingReferenceEntriesIfNecessaryAsync().ConfigureAwait(false);
            await CreateNoResultsFoundEntryIfNecessaryAsync().ConfigureAwait(false);

            _tableDataSink.IsStable = true;
        }

        // No references found to
        async Task CreateMissingReferenceEntriesIfNecessaryAsync() {

            foreach (var definition in _definitionEntries.Where(definition => !HasReferences(definition))) {

                await OnReferenceFoundAsync(ReferenceItem.NoReferencesFoundTo(definition.DefinitionItem));
            }

            bool HasReferences(DefinitionEntry definitionEntry) {
                return _entries.Any(r => Equals(r.Definition, definitionEntry));
            }
        }

        // search found no results
        async Task CreateNoResultsFoundEntryIfNecessaryAsync() {

            if (_definitionEntries.Length > 0) {
                return;
            }

            var msg             = "Search found no results";
            var definition      = DefinitionItem.CreateSimpleItem(msg);
            var definitionEntry = EnsureDefinitionEntry(definition, ImageMonikers.Information);

            var entry = SimpleTextEntry.Create(Presenter, definitionEntry, msg);

            await OnEntryFoundAsync(entry);

        }

        public Task SetSearchTitleAsync(string title) {
            _findReferencesWindow.Title = title;
            return Task.CompletedTask;
        }

        protected readonly IWpfTableControl2 TableControl;

        protected void NotifyChange() => _tableDataSink.FactorySnapshotChanged(this);

        private void OnFindReferencesWindowClosed(object sender, EventArgs e) {
            //Presenter.AssertIsForeground();
            CancelSearch();

            _findReferencesWindow.Closed -= OnFindReferencesWindowClosed;
            //TableControl.GroupingsChanged -= OnTableControlGroupingsChanged;
        }

        public IDisposable Subscribe(ITableDataSink sink) {
            //Presenter.AssertIsForeground();

            Debug.Assert(_tableDataSink == null);
            _tableDataSink = sink;

            _tableDataSink.AddFactory(this, removeAllFactories: true);
            _tableDataSink.IsStable = false;

            return this;
        }

        public string SourceTypeIdentifier => FindAllReferencesSourceTypeIdentifier;
        public string Identifier           => FindAllReferencesIdentifier;
        public string DisplayName          => "Nav Data Source";

        public const string FindAllReferencesSourceTypeIdentifier = StandardTableDataSources2.FindAllReferencesTableDataSource;
        public const string FindAllReferencesIdentifier           = nameof(FindAllReferencesIdentifier);

        public void Dispose() {
            _findReferencesWindow.Manager.RemoveSource(this);
        }

        public ITableEntriesSnapshot GetCurrentSnapshot() {

            lock (_gate) {
                // If our last cached snapshot matches our current version number, then we
                // can just return it.  Otherwise, we need to make a snapshot that matches
                // our version.
                if (_lastSnapshot?.VersionNumber != CurrentVersionNumber) {
                    // If we've been cleared, then just return an empty list of entries.
                    // Otherwise return the appropriate list based on how we're currently
                    // grouping.
                    //var entries = _cleared
                    //    ? ImmutableList<Entry>.Empty
                    //    : _currentlyGroupingByDefinition
                    //        ? EntriesWhenGroupingByDefinition
                    //        : EntriesWhenNotGroupingByDefinition;

                    _lastSnapshot = new TableEntriesSnapshot(this, CurrentVersionNumber, _entries);
                }

                return _lastSnapshot;
            }
        }

        public ITableEntriesSnapshot GetSnapshot(int versionNumber) {
            lock (_gate) {
                if (_lastSnapshot?.VersionNumber == versionNumber) {
                    return _lastSnapshot;
                }

                if (versionNumber == CurrentVersionNumber) {
                    return GetCurrentSnapshot();
                }
            }

            // We didn't have this version.  Notify the sinks that something must have changed
            // so that they call back into us with the latest version.
            NotifyChange();
            return null;
        }

        public int CurrentVersionNumber { get; protected set; }

        private void CancelSearch() {
            //Presenter.AssertIsForeground();
            _cancellationTokenSource.Cancel();
        }

    }

}