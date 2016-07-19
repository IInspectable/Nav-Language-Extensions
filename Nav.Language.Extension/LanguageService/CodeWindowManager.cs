#region Using Directives

using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TextManager.Interop;

using Pharmatechnik.Nav.Language.Extension.NavigationBar;
using Pharmatechnik.Nav.Utilities.Logging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.LanguageService {

    class CodeWindowManager : IVsCodeWindowManager {

        static readonly Logger Logger = Logger.Create<CodeWindowManager>();

        readonly IVsCodeWindow _codeWindow;
        readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;
        readonly IServiceProvider _serviceProvider;

        DropdownBarClient _dropdownBarClient;

        public CodeWindowManager(IVsCodeWindow codeWindow, IServiceProvider serviceProvider) {
            _codeWindow       = codeWindow;
            _serviceProvider  = serviceProvider;

            var componentModel = (IComponentModel)_serviceProvider.GetService(typeof(SComponentModel));
            _editorAdaptersFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();
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

            IVsTextView textView;
            _codeWindow.GetPrimaryView(out textView);

            if(textView == null) {
                Logger.Warn($"{nameof(AddDropdownBar)}: Unable to get primary view");
                return;
            }

            var wpfTextView = _editorAdaptersFactoryService.GetWpfTextView(textView);
            if (wpfTextView == null) {
                Logger.Warn($"{nameof(AddDropdownBar)}: Unable to get IWpfTextView");
                return;
            }

            var dropdownBarClient = new DropdownBarClient(wpfTextView.TextBuffer, dropdownManager, _codeWindow, _serviceProvider);

            #if ShowMemberCombobox
            var hr = dropdownManager.AddDropdownBar(cCombos: 3, pClient: dropdownBarClient);
            #else
            var hr = dropdownManager.AddDropdownBar(cCombos: 2, pClient: dropdownBarClient);
            #endif
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