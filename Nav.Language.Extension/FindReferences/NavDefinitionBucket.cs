#region Using Directives

using System;
using Microsoft.VisualStudio.Shell.FindAllReferences;
using Microsoft.VisualStudio.Shell.TableControl;

using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.FindReferences;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    class NavDefinitionBucket: DefinitionBucket {

        public NavDefinitionBucket(FindReferencesPresenter presenter,
                                   DefinitionEntry definitionEntry, 
                                   string sourceTypeIdentifier, 
                                   string identifier, object tooltip = null, 
                                   StringComparer comparer = null, 
                                   bool expandedByDefault = true)
            : base(definitionEntry.FullText, sourceTypeIdentifier, identifier, tooltip, comparer, expandedByDefault) {
            Presenter = presenter;
            DefinitionEntry = definitionEntry;
        }

        public FindReferencesPresenter Presenter { get; }
        public DefinitionEntry DefinitionEntry { get; }

        public override bool TryGetValue(string key, out object content) {
            content = null;
            switch (key) {
                case StandardTableKeyNames2.DefinitionIcon:
                    content = ImageMonikers.FromSymbol(DefinitionEntry.Symbol);
                    break;
                case StandardTableKeyNames2.TextInlines:
                    content = Presenter.ToInlines(DefinitionEntry.DisplayParts, true);
                    break;
            }

            return content!=null;
        }

        //public override bool TryCreateColumnContent(out FrameworkElement content) {
        //    content = Presenter.ToTextBlock(DefinitionEntry.DisplayParts);
        //    if (content != null) {
        //        return true;
        //    }
        //    return base.TryCreateColumnContent(out content);
        //}

    }

}