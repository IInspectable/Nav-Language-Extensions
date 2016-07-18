#region 

using System.Windows.Forms;
using Microsoft.VisualStudio.Imaging;

using Pharmatechnik.Nav.Language.Extension.QuickInfo;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    static class NavigationImages {

        public static class Index {
            public const int ProjectNode = 0;
            public const int TaskDeclaration = 1;
            public const int TaskDefinition = 2;
            public const int TriggerSymbol = 3;
        }
        
        public static ImageList CreateImageList() {

            var imgageList = new ImageList { ImageSize = new System.Drawing.Size(16, 16) };

            imgageList.Images.Add(NavLanguagePackage.GetBitmap(KnownMonikers.CSProjectNode));
            imgageList.Images.Add(NavLanguagePackage.GetBitmap(SymbolImageMonikers.TaskDeclaration));
            imgageList.Images.Add(NavLanguagePackage.GetBitmap(SymbolImageMonikers.TaskDefinition));
            imgageList.Images.Add(NavLanguagePackage.GetBitmap(SymbolImageMonikers.SignalTrigger));

            return imgageList;
        }
    }
}