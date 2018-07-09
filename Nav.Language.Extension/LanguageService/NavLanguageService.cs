using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Pharmatechnik.Nav.Language.Extension.LanguageService {

    [Guid(GuidList.NavLanguage)]
    public class NavLanguageService: Microsoft.VisualStudio.Package.LanguageService {

        public NavLanguageService(Extension.NavLanguagePackage package) {
            Package = package;

            ThreadHelper.ThrowIfNotOnUIThread();
            SetSite(package);

        }

        public Extension.NavLanguagePackage Package { get; }

        public override LanguagePreferences GetLanguagePreferences() {
            var preferences = new LanguagePreferences(Site, typeof(NavLanguageService).GUID, Name);
            preferences.Init();

            preferences.EnableCodeSense             = true;
            preferences.EnableMatchBraces           = true;
            preferences.EnableMatchBracesAtCaret    = true;
            preferences.EnableShowMatchingBrace     = true;
            preferences.EnableCommenting            = true;
            preferences.HighlightMatchingBraceFlags = _HighlightMatchingBraceFlags.HMB_USERECTANGLEBRACES;
            preferences.LineNumbers                 = true;
            preferences.MaxErrorMessages            = 100;
            preferences.AutoOutlining               = true;
            preferences.MaxRegionTime               = 2000;
            preferences.InsertTabs                  = false;
            preferences.IndentSize                  = 4;
            preferences.ShowNavigationBar           = true;
            preferences.EnableAsyncCompletion       = true;

            preferences.WordWrap       = false;
            preferences.WordWrapGlyphs = true;

            preferences.AutoListMembers      = true;
            preferences.EnableQuickInfo      = true;
            preferences.ParameterInformation = true;
            preferences.HideAdvancedMembers  = false;

            return preferences;
        }

        public override IScanner GetScanner(IVsTextLines buffer) {
            return null;
        }

        public override AuthoringScope ParseSource(ParseRequest req) {
            return null;
        }

        public override string GetFormatFilterList() {
            return $"Nav File (*{NavLanguageContentDefinitions.FileExtension})|*{NavLanguageContentDefinitions.FileExtension}";
        }

        public override string Name => NavLanguageContentDefinitions.LanguageName;

        public override TypeAndMemberDropdownBars CreateDropDownHelper(IVsTextView forView)
        {
            if (Preferences.ShowNavigationBar) {
                return new NavigationBar.NavigationBar(this, forView);
            }

            return null;
        }
    }

}