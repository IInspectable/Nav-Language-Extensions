#region Using Directives

using System;
using System.Collections.Immutable;
using System.Windows;

using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.FindReferences;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class TableEntriesSnapshot: WpfTableEntriesSnapshotBase {

        readonly FindReferencesContext          _context;
        readonly ImmutableArray<ReferenceEntry> _entries;

        public TableEntriesSnapshot(FindReferencesContext context, int versionNumber, ImmutableArray<ReferenceEntry> entries) {
            _context      = context;
            _entries      = entries;
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
            content = GetValue(_entries[index], keyName);

            return content != null;
        }

        private object GetValue(ReferenceEntry entry, string keyName) {
            switch (keyName) {
                case StandardTableKeyNames2.Definition:
                    return new NavDefinitionBucket(Presenter, entry.Definition, _context.SourceTypeIdentifier, _context.Identifier);
                case StandardTableKeyNames2.DefinitionIcon:
                    return ImageMonikers.TaskDefinition;
                case StandardTableColumnDefinitions.DocumentName:
                    return entry.Location.FilePath;
                case StandardTableKeyNames.Line:
                    return entry.Location.StartLine;
                case StandardTableKeyNames.Column:
                    return entry.Location.StartCharacter;
                case StandardTableKeyNames.ProjectName:
                    return NavLanguagePackage.GetContainingProject(entry.Location.FilePath)?.Name ?? "Miscellaneous Files";
                case StandardTableKeyNames.ProjectGuid:
                    return Guid.NewGuid();
                case StandardTableKeyNames.Text:
                    return entry.Text;
                case StandardTableKeyNames2.TextInlines:
                    return Presenter.ToInlines(entry.DisplayParts);
            }

            return null;
        }

        public override bool TryCreateColumnContent(
            int index, string columnName, bool singleColumnView, out FrameworkElement content) {
            content = null;

            return false;
        }

    }

}