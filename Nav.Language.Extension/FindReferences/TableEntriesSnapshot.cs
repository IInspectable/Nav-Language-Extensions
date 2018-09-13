#region Using Directives

using System;
using System.Windows;

using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class TableEntriesSnapshot: WpfTableEntriesSnapshotBase {

        private readonly FindUsagesContext _context;

        public TableEntriesSnapshot(FindUsagesContext context, int versionNumber) {
            _context      = context;
            VersionNumber = versionNumber;
        }

        public override int VersionNumber { get; }

        public override int Count => 1;

        public override int IndexOf(int currentIndex, ITableEntriesSnapshot newSnapshot) {
            // We only add items to the end of our list, and we never reorder.
            // As such, any index in us will map to the same index in any newer snapshot.
            return currentIndex;
        }

        public override bool TryGetValue(int index, string keyName, out object content) {
            content = GetValue(keyName);
            Console.WriteLine(keyName);

            return content != null;
        }

        private object GetValue(string keyName) {
            string fileName = @"C:\ws\XTplus\z_ahifi\XTplusApplication\src\XTplus.Verkauf\Pflegehilfsmittel\PflegehilfsmittelAbrechnungDruck.nav";
            switch (keyName) {
                case StandardTableKeyNames2.Definition:
                    return new NavDefinitionBucket("task Foo", _context.SourceTypeIdentifier, _context.Identifier);

                case StandardTableKeyNames2.DefinitionIcon:
                    return ImageMonikers.TaskDefinition;

                case StandardTableColumnDefinitions.DocumentName:
                    return fileName;
                case StandardTableKeyNames.Line:
                    return 1;
                case StandardTableKeyNames.Column:
                    return 1;
                case StandardTableKeyNames.ProjectName:
                    return NavLanguagePackage.GetContainingProject(fileName)?.Name ?? "Miscellaneous Files";
                case StandardTableKeyNames.ProjectGuid:
                    return Guid.NewGuid();
                case StandardTableKeyNames.Text:
                    return "Hi Text!";
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