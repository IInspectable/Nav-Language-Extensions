#region Using Directives

using Microsoft.VisualStudio.Shell.FindAllReferences;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.FindReferences;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class DefinitionEntry: DefinitionBucket {

        public DefinitionEntry(FindReferencesPresenter presenter,
                               DefinitionItem definitionItem,
                               bool expandedByDefault = true)
            : base(definitionItem.FullText,
                   FindReferencesContext.FindAllReferencesSourceTypeIdentifier,
                   FindReferencesContext.FindAllReferencesIdentifier,
                   tooltip: null,
                   comparer: null,
                   expandedByDefault: expandedByDefault) {

            Presenter      = presenter;
            DefinitionItem = definitionItem;
        }

        public FindReferencesPresenter Presenter      { get; }
        public DefinitionItem          DefinitionItem { get; }

        public override bool TryGetValue(string key, out object content) {
            content = GetValue(key);
            return content != null;
        }

        public object GetValue(string key) {
            switch (key) {
                case StandardTableKeyNames2.DefinitionIcon:
                    return ImageMonikers.FromSymbol(DefinitionItem.Symbol);
                case StandardTableKeyNames2.TextInlines:
                    return Presenter.ToInlines(DefinitionItem.DisplayParts, (run, _, __) => Presenter.SetBold(run));
                case StandardTableKeyNames.DocumentName:
                    return DefinitionItem.Location.FilePath;
                case StandardTableKeyNames.Line:
                    return DefinitionItem.Location.StartLine;
                case StandardTableKeyNames.Column:
                    return DefinitionItem.Location.StartCharacter;
            }

            return null;
        }

    }

}