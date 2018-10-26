#region Using Directives

using System;
using System.Drawing;

using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    static class NavigationBarImages {

        public static class Index {
            public const int ProjectNode     = 0;
            public const int TaskDeclaration = 1;
            public const int TaskDefinition  = 2;
            public const int TriggerSymbol   = 3;
        }

        static IImageHandle _imageListHandle;

        public static IntPtr GetImageList(Color backgroundColor, IVsImageService2 imageService) {

            ThreadHelper.ThrowIfNotOnUIThread();

            EnsureImageListHandle(imageService);

            IntPtr hImageList = NavLanguagePackage.GetImageList(_imageListHandle.Moniker, backgroundColor);

            return hImageList;
        }

        static void EnsureImageListHandle(IVsImageService2 imageService) {
            
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_imageListHandle != null) {
                return;
            }
            
            var imageList = new ImageMonikerImageList(
                ImageMonikers.ProjectNode,
                ImageMonikers.TaskDeclaration,
                ImageMonikers.TaskDefinition,
                ImageMonikers.SignalTrigger);

            _imageListHandle = imageService.AddCustomImageList(imageList);
        }
    }
}