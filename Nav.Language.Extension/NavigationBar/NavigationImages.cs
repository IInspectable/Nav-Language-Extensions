#region

using System;
using System.Drawing;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.Extension.Common;
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

        static IImageHandle _imageListHandle;

        public static IntPtr GetImageList(Color backgroundColor, IVsImageService2 imageService) {

            InitializeImageMonikers(imageService);

            IntPtr hImageList = NavLanguagePackage.GetImageList(_imageListHandle.Moniker, backgroundColor);

            return hImageList;          
        }

        static void InitializeImageMonikers(IVsImageService2 imageService) {

            if (_imageListHandle!=null) {
                return;
            }

            var taskDelarationMoniker = GetCompositedImageMoniker(imageService,
                CreateLayer(SymbolImageMonikers.TaskDeclaration),
                CreateLayer(SymbolImageMonikers.TaskDeclarationOverlay));

            var imageList = new ImageMonikerImageList(
                    KnownMonikers.CSProjectNode, 
                    taskDelarationMoniker, 
                    SymbolImageMonikers.TaskDefinition, 
                    SymbolImageMonikers.SignalTrigger);

            _imageListHandle = imageService.AddCustomImageList(imageList);
        }

        static ImageCompositionLayer CreateLayer(
            ImageMoniker imageMoniker,
            int virtualWidth   = 16,
            int virtualYOffset = 0,
            int virtualXOffset = 0) {

            return new ImageCompositionLayer {
                VirtualWidth        = virtualWidth,
                VirtualHeight       = 16,
                ImageMoniker        = imageMoniker,
                HorizontalAlignment = (uint)_UIImageHorizontalAlignment.IHA_Left,
                VerticalAlignment   = (uint)_UIImageVerticalAlignment.IVA_Top,
                VirtualXOffset      = virtualXOffset,
                VirtualYOffset      = virtualYOffset,
            };
        }

        static ImageMoniker GetCompositedImageMoniker(IVsImageService2 imageService, params ImageCompositionLayer[] layers) {
            
            var imageHandle = imageService.AddCustomCompositeImage(
                virtualWidth : 16, 
                virtualHeight: 16,
                layerCount   : layers.Length, 
                layers       : layers);

            var moniker = imageHandle.Moniker;

            return moniker;
        }
    }
}