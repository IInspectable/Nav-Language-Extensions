#region Using Directives

using System.Collections.Immutable;
using System.Windows;

using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class TableEntriesSnapshot: WpfTableEntriesSnapshotBase {

        readonly FindReferencesContext _context;
        readonly ImmutableArray<Entry> _entries;

        public TableEntriesSnapshot(FindReferencesContext context, int versionNumber, ImmutableArray<Entry> items) {
            _context      = context;
            _entries      = items;
            VersionNumber = versionNumber;

        }

        public FindReferencesPresenter Presenter => _context.Presenter;

        public override int VersionNumber { get; }

        public override int Count => _entries.Length;

        public override int IndexOf(int currentIndex, ITableEntriesSnapshot newSnapshot) {
            // We only add items to the end of our list, and we never reorder.
            // As such, any index in us will map to the same index in any newer snapshot.
            return currentIndex;
        }

        public override bool TryGetValue(int index, string keyName, out object content) {
            content = _entries[index].GetValue(keyName);
            return content != null;
        }

        public override bool TryCreateColumnContent(int index, string columnName, bool singleColumnView, out FrameworkElement content) {

            if (columnName == StandardTableColumnDefinitions2.LineText) {

                content = _entries[index].TryCreateColumnContent();

                return content != null;
            }

            return base.TryCreateColumnContent(index, columnName, singleColumnView, out content);

        }

    }

}