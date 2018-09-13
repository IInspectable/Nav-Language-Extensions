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

        public bool StartSearch() {

            var window = _vsFindAllReferencesService.StartSearch("Foo");

            window.Title = "Test ";

            var _ = new FindUsagesContext(window);

            return true;
        }

    }

}