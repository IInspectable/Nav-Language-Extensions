#region Using Directives

using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Pharmatechnik.Nav.Language.Extension.NavigationBar;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.LanguageService {

    class CodeWindowManager : IVsCodeWindowManager {

        readonly IVsCodeWindow _codeWindow;
        readonly IWpfTextView _textView;
        readonly IServiceProvider _serviceProvider;

        DropdownBarClient _dropdownBarClient;

        public CodeWindowManager(IVsCodeWindow codeWindow, IWpfTextView textView, IServiceProvider serviceProvider) {
            _codeWindow      = codeWindow;
            _textView        = textView;
            _serviceProvider = serviceProvider;
        }

        public int AddAdornments() {

            AddOrRemoveDropdown(enabled: true);

            return VSConstants.S_OK;
        }

        void AddOrRemoveDropdown(bool enabled) {

            // ReSharper disable once SuspiciousTypeConversion.Global
            var dropdownManager = _codeWindow as IVsDropdownBarManager;

            if (dropdownManager == null) {
                return;
            }

            if (enabled) {
                var existingDropdownBar = GetDropdownBar(dropdownManager);
                if (existingDropdownBar != null) {
                    
                    // Check if the existing dropdown is already one of ours, and do nothing if it is.
                    if (_dropdownBarClient != null &&
                        _dropdownBarClient == GetDropdownBarClient(existingDropdownBar)) {
                        return;
                    }

                    // Not ours, so remove the old one so that we can add ours.
                    RemoveDropdownBar(dropdownManager);
                } 

                AddDropdownBar(dropdownManager);
            } else {
                RemoveDropdownBar(dropdownManager);
            }
        }

        void AddDropdownBar(IVsDropdownBarManager dropdownManager) {
           
            var dropdownBarClient = new DropdownBarClient(_textView, dropdownManager, _codeWindow, _serviceProvider);
           
            // TODO: Entscheiden, ob die "Member Combo" Sinn macht, oder nicht. Bis dahin bleibt sie erst mal ausgeblendet
            var hr = dropdownManager.AddDropdownBar(cCombos: 2, pClient: dropdownBarClient);

            if (ErrorHandler.Failed(hr)) {
                ErrorHandler.ThrowOnFailure(hr);
            }

            _dropdownBarClient = dropdownBarClient;
        }

        void RemoveDropdownBar(IVsDropdownBarManager dropdownManager) {
            if (ErrorHandler.Succeeded(dropdownManager.RemoveDropdownBar())) {

                _dropdownBarClient?.Dispose();
                _dropdownBarClient = null;
            }
        }

        public int RemoveAdornments() {

            AddOrRemoveDropdown(enabled: false);

            return VSConstants.S_OK;
        }

        public int OnNewView(IVsTextView pView) {
            return VSConstants.S_OK;
        }

        static IVsDropdownBar GetDropdownBar(IVsDropdownBarManager dropdownManager) {
            IVsDropdownBar existingDropdownBar;
            ErrorHandler.ThrowOnFailure(dropdownManager.GetDropdownBar(out existingDropdownBar));
            return existingDropdownBar;
        }

        static IVsDropdownBarClient GetDropdownBarClient(IVsDropdownBar dropdownBar) {
            IVsDropdownBarClient dropdownBarClient;
            ErrorHandler.ThrowOnFailure(dropdownBar.GetClient(out dropdownBarClient));
            return dropdownBarClient;
        }
    }
}