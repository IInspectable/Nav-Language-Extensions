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
            Definition    = new DefinitionEntry(Presenter, ReferenceItem.Definition, ReferenceItem.Definition.ExpandedByDefault);
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
                case StandardTableKeyNames.Line when ReferenceItem.Location.StartLine > 0:
                    return ReferenceItem.Location.StartLine;
                case StandardTableKeyNames.Column when ReferenceItem.Location.StartCharacter > 0:
                    return ReferenceItem.Location.StartCharacter;
                case StandardTableKeyNames.ProjectName:
                    return ReferenceItem.ProjectName;

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

                var highlightExtent = ReferenceItem.TextHighlightExtent;

                if (!highlightExtent.IsMissing &&
                    position == highlightExtent.Start) {

                    Presenter.HighlightBackground(run);
                }

            });
        }

        ToolTip CreateToolTip() {

            return Presenter.CreateToolTip(ToolTipPartsToTextBlock());

        }

        TextBlock ToolTipPartsToTextBlock() {

            return Presenter.ToTextBlock(ReferenceItem.ToolTipParts, (run, part, position) => {

                var highlightExtent = ReferenceItem.ToolTipHighlightExtent;

                if (!highlightExtent.IsMissing &&
                    position == highlightExtent.Start) {

                    Presenter.HighlightBackground(run);

                }

            });
        }

    }

}