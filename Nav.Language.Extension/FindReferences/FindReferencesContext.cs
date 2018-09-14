#region Using Directives

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell.FindAllReferences;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

using Pharmatechnik.Nav.Language.FindReferences;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class FindReferencesContext: IFindReferencesContext, ITableDataSource, ITableEntriesSnapshotFactory {

        readonly object _objectLock = new object();

        readonly CancellationTokenSource  _cancellationTokenSource;
        readonly IFindAllReferencesWindow _findReferencesWindow;

        TableEntriesSnapshot _lastSnapshot;
        ITableDataSink       _tableDataSink;

        ImmutableArray<ReferenceEntry> _entries = ImmutableArray<ReferenceEntry>.Empty;

        public FindReferencesContext(FindReferencesPresenter presenter, IFindAllReferencesWindow findReferencesWindow) {

            Presenter = presenter;

            _cancellationTokenSource = new CancellationTokenSource();

            _findReferencesWindow = findReferencesWindow;

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

        public Task OnReferenceFoundAsync(ReferenceEntry entry) {

            lock (_objectLock) {
                _entries = _entries.Add(entry);
                CurrentVersionNumber++;
            }

            NotifyChange();

            return Task.CompletedTask;
        }

        public Task OnCompletedAsync() {

            _tableDataSink.IsStable = true;

            return Task.CompletedTask;
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

            lock (_objectLock) {
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
            lock (_objectLock) {
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