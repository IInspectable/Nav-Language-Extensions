#region Using Directives

using System.Windows;
using System.Windows.Controls;

using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

using Pharmatechnik.Nav.Language.Extension.Utilities;
using Pharmatechnik.Nav.Language.FindReferences;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences; 

class ReferenceEntry: Entry {

    ReferenceEntry(FindReferencesPresenter presenter,
                   ReferenceItem referenceItem,
                   DefinitionEntry definitionEntry,
                   ProjectInfo projectInfo):
        base(presenter, definitionEntry) {

        ReferenceItem = referenceItem;
        ProjectInfo   = projectInfo;
    }

    public static ReferenceEntry Create(FindReferencesPresenter presenter,
                                        DefinitionEntry definitionEntry,
                                        ReferenceItem referenceItem,
                                        ProjectInfo projectInfo
    ) {

        return new ReferenceEntry(presenter, referenceItem, definitionEntry, projectInfo);
    }

    public ReferenceItem ReferenceItem { get; }
    public ProjectInfo   ProjectInfo   { get; }

    public override string Text => ReferenceItem.Text;

    public override object GetValue(string keyName) {
        switch (keyName) {

            case StandardTableColumnDefinitions.DocumentName:
                return ReferenceItem.Location.FilePath;
            case StandardTableKeyNames.Line when ReferenceItem.Location.StartLine > 0:
                return ReferenceItem.Location.StartLine;
            case StandardTableKeyNames.Column when ReferenceItem.Location.StartCharacter > 0:
                return ReferenceItem.Location.StartCharacter;
            case StandardTableKeyNames.ProjectName:
                return ProjectInfo.ProjectName;
            // Wird zum Filtern nach z.B. "Current Project" benötigt
            case StandardTableKeyNames.ProjectGuid:
                return ProjectInfo.ProjectGuid;

        }

        return base.GetValue(keyName);
    }

    public override FrameworkElement TryCreateColumnContent() {

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