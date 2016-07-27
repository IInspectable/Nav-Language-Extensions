#region Using Directives

using System.ComponentModel.Design;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.LanguageService {
    [Guid(GuidList.NavLanguage)]
    public class NavLanguageInfo : IVsLanguageInfo {

        readonly IServiceContainer _serviceContainer;

        public NavLanguageInfo(IServiceContainer serviceContainer) {
            _serviceContainer = serviceContainer;
        }
        
        public int GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager ppCodeWinMgr) {

            ppCodeWinMgr = new CodeWindowManager(pCodeWin, _serviceContainer);
            
            return VSConstants.S_OK;
        }

        public int GetColorizer(IVsTextLines pBuffer, out IVsColorizer ppColorizer) {
            ppColorizer = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetFileExtensions(out string pbstrExtensions) {
            pbstrExtensions = NavLanguageContentDefinitions.FileExtension;
            return VSConstants.S_OK;
        }

        public int GetLanguageName(out string bstrName) {
            bstrName = NavLanguageContentDefinitions.LanguageName;
            return VSConstants.S_OK;
        }
    }
}