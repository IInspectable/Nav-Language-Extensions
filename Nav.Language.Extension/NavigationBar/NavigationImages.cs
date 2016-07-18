#region 

using System.Drawing;
using System.Windows.Forms;

using Microsoft.VisualStudio.Imaging;

using Pharmatechnik.Nav.Language.Extension.QuickInfo;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    static class NavigationImages {

        public static class Index {
            public const int ProjectNode     = 0;
            public const int TaskDeclaration = 1;
            public const int TaskDefinition  = 2;
            public const int TriggerSymbol   = 3;
        }

        public static ImageList CreateImageList(Color backgroundColor) {

            var baseImage = NavLanguagePackage.GetBitmap(SymbolImageMonikers.TaskDeclaration, backgroundColor);
            var overlay   = NavLanguagePackage.GetBitmap(KnownMonikers.ReferencedElement, backgroundColor);

            Bitmap taskDeclarationImage = new Bitmap(baseImage);
            using (var grfx = Graphics.FromImage(taskDeclarationImage)) {
                grfx.DrawImage(overlay, 0, 0);
            }

            var imgageList = new ImageList {ImageSize = new Size(16, 16)};

            imgageList.Images.Add(NavLanguagePackage.GetBitmap(KnownMonikers.CSProjectNode, backgroundColor));
            imgageList.Images.Add(taskDeclarationImage);
            imgageList.Images.Add(NavLanguagePackage.GetBitmap(SymbolImageMonikers.TaskDefinition, backgroundColor));
            imgageList.Images.Add(NavLanguagePackage.GetBitmap(SymbolImageMonikers.SignalTrigger, backgroundColor));

            return imgageList;
        }
    }
}