#region Using Directives

using System.Windows;
using System.Windows.Controls;

using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

using Pharmatechnik.Nav.Language.FindReferences;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class ReferenceEntry {

        public ReferenceEntry(FindReferencesPresenter presenter, ReferenceItem referenceItem) {
            Presenter     = presenter;
            ReferenceItem = referenceItem;
            Definition    = new DefinitionEntry(Presenter, ReferenceItem.Definition);
        }

        public FindReferencesPresenter Presenter     { get; }
        public ReferenceItem           ReferenceItem { get; }
        public DefinitionEntry         Definition    { get; }

        public object GetValue(string keyName) {
            switch (keyName) {
                case StandardTableKeyNames.Text:
                    // Wird für die Suche verwendet...
                    return ReferenceItem.Text;
                case StandardTableKeyNames2.Definition:
                    return Definition;
                case StandardTableColumnDefinitions.DocumentName:
                    return ReferenceItem.Location.FilePath;
                case StandardTableKeyNames.Line:
                    return ReferenceItem.Location.StartLine;
                case StandardTableKeyNames.Column:
                    return ReferenceItem.Location.StartCharacter;
                case StandardTableKeyNames.ProjectName:
                    return "Miscellaneous Files";
                    //return NavLanguagePackage.GetContainingProject(ReferenceItem.Location.FilePath)?.Name ?? "Miscellaneous Files";
                //case StandardTableKeyNames.ProjectGuid:
                //    return Guid.NewGuid(); // Brauchen wir die? Evtl. zum Gruppieren?

            }

            return null;
        }

        public FrameworkElement CreatLineContent(bool singleColumnView) {

            var content = LinePartsToTextBlock();
            LazyTooltip.AttachTo(content, CreateToolTip);

            return content;
        }

        TextBlock LinePartsToTextBlock() {

            return Presenter.ToTextBlock(ReferenceItem.TextParts, (run, part, position) => {

                if (position == ReferenceItem.TextHighlightExtent.Start) {

                    Presenter.HighlightBackground(run);
                }

            });
        }

        ToolTip CreateToolTip() {

            return Presenter.CreateToolTip(PreviewPartsToTextBlock());

        }

        TextBlock PreviewPartsToTextBlock() {

            return Presenter.ToTextBlock(ReferenceItem.PreviewParts, (run, part, position) => {

                if (position == ReferenceItem.PreviewHighlightExtent.Start) {

                    Presenter.HighlightBackground(run);

                }

            });
        }

    }

}