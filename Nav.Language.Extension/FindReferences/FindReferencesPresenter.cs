#region Using Directives

using System.ComponentModel.Composition;

using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.FindAllReferences;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    [Export(typeof(FindReferencesPresenter))]
    class FindReferencesPresenter {

        readonly IFindAllReferencesService _vsFindAllReferencesService;

        [ImportingConstructor]
        public FindReferencesPresenter(SVsServiceProvider serviceProvider) {

            _vsFindAllReferencesService = (IFindAllReferencesService) serviceProvider.GetService(typeof(SVsFindAllReferences));
            Assumes.Present(_vsFindAllReferencesService);
        }

        private const string Title = "Find References";

        public FindReferencesContext StartSearch() {

            var window = _vsFindAllReferencesService.StartSearch(Title);
            var context = new FindReferencesContext(window);

            return context;
        }

    }

}