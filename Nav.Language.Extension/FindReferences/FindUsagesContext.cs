#region Using Directives

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell.FindAllReferences;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class FindUsagesContext: ITableDataSource, ITableEntriesSnapshotFactory {

        private readonly object _objectLock = new object();

        readonly CancellationTokenSource  _cancellationTokenSource;
        readonly IFindAllReferencesWindow _findReferencesWindow;

        TableEntriesSnapshot _lastSnapshot;
        ITableDataSink       _tableDataSink;

        public FindUsagesContext(
            IFindAllReferencesWindow findReferencesWindow) {

            _cancellationTokenSource = new CancellationTokenSource();

            _findReferencesWindow = findReferencesWindow;

            TableControl = (IWpfTableControl2) findReferencesWindow.TableControl;
            //TableControl.GroupingsChanged += OnTableControlGroupingsChanged;

            _findReferencesWindow.Closed += OnFindReferencesWindowClosed;

            _findReferencesWindow.Manager.AddSource(this);

            var _ = Task.Run(async () => {
                await Task.Delay(500);

                NotifyChange();

                _tableDataSink.IsStable = true;

            });
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

        public string SourceTypeIdentifier => NavFindUsagesTableDataSourceSourceTypeIdentifier;
        public string Identifier           => NavFindUsagesTableDataSourceIdentifier;
        public string DisplayName          => "Nav Data Source";

        public const string NavFindUsagesTableDataSourceSourceTypeIdentifier = nameof(NavFindUsagesTableDataSourceSourceTypeIdentifier);
        public const string NavFindUsagesTableDataSourceIdentifier           = nameof(NavFindUsagesTableDataSourceIdentifier);

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

                    _lastSnapshot = new TableEntriesSnapshot(this, CurrentVersionNumber);
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