#region Using Directives

using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.LanguageService {

    [Guid(NavLanguagePackage.Guids.LanguageGuidString)]
    public class NavLanguageService: IVsLanguageInfo {

        

        readonly NavLanguagePackage _package;

        private NavLanguagePreferences _preferences;

        public NavLanguageService(NavLanguagePackage package) {
            _package = package;
        }

        public NavLanguagePreferences Preferences {
            get {
                if (_preferences == null) {
                    _preferences = new NavLanguagePreferences(_package, typeof(NavLanguageService).GUID, NavLanguageContentDefinitions.LanguageName);
                    _preferences.Init();
                }

                return _preferences;
            }
        }

        public int GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager ppCodeWinMgr) {

            ppCodeWinMgr = new NavCodeWindowManager(this, _package, pCodeWin);

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