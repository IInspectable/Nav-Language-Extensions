#region Using Directives

using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Imaging.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    interface IDialogService {

        string ShowInputDialog(string promptText, string title = null, string defaultResonse = null, ImageMoniker iconMoniker = default(ImageMoniker), Func<string, string> validator = null);
    }

    [Export(typeof(IDialogService))]
    class DialogService: IDialogService {

        public string ShowInputDialog(string promptText, string title = null, string defaultResonse = null, ImageMoniker iconMoniker = new ImageMoniker(), Func<string, string> validator = null) {

            var viewModel = new InputDialogViewModel {
                PromptText  = promptText,
                Title       = title,
                Text        = defaultResonse,
                IconMoniker = iconMoniker,
                Validator   = validator
            };

            var dlg = new InputDialog(viewModel);
            if (dlg.ShowModal() == false) {
                return null;
            }

            return viewModel.Text;
        }
    }
}