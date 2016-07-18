#region 

using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {
    abstract class NavigationItemBuilderBase: SymbolVisitor {
        protected NavigationItemBuilderBase() {
            NavigationItems = new List<NavigationItem>();
        }

        public List<NavigationItem> NavigationItems { get; }

        protected static ImmutableList<NavigationItem> BuildCore(CodeGenerationUnit codeGenerationUnit, NavigationItemBuilderBase builder) {

            foreach (var symbol in codeGenerationUnit.Symbols) {
                builder.Visit(symbol);
            }

            var items =  builder.NavigationItems
                                .OrderBy(ni=>ni.Location.Start)
                                .ToImmutableList();

            return items;
        }       
    }
}