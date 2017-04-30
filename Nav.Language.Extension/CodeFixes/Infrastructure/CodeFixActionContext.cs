#region Using Directives

using System.ComponentModel.Composition;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [Export(typeof(CodeFixActionContext))]
    class CodeFixActionContext {
        
        [ImportingConstructor]
        public CodeFixActionContext(IWaitIndicator waitIndicator,
            ITextChangeService textChangeService, 
            IDialogService dialogService) {

            WaitIndicator     = waitIndicator;
            TextChangeService = textChangeService;
            DialogService     = dialogService;
        }

        public IWaitIndicator WaitIndicator { get; }
        public ITextChangeService TextChangeService { get; }
        public IDialogService DialogService { get; }       
    }
}