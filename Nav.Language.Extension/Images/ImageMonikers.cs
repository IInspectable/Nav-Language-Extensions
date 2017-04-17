#region Using Directives

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Images {

    public static class ImageMonikers {

        public static ImageMoniker ProjectNode {
            get { return KnownMonikers.CSProjectNode; }
        }

        #region Analysis

        public static ImageMoniker WaitingForAnalysis {
            get { return KnownMonikers.Loading; }
        }

        public static ImageMoniker AnalysisOK {
            get { return KnownMonikers.StatusOK; }
        }

        public static ImageMoniker AnalysisWarning {
            get { return KnownMonikers.StatusWarning; }
        }

        public static ImageMoniker AnalysisError {
            get { return KnownMonikers.StatusError; }
        }

        #endregion

        #region GoTo

        /// <summary>
        /// Nav file --> C# file
        /// </summary>
        public static ImageMoniker GoToDeclaration {
            get { return KnownMonikers.GoToDefinition; }
        }

        /// <summary>
        /// C# file --> Nav file
        /// </summary>
        public static ImageMoniker GoToDefinition {
            get { return KnownMonikers.GoToDeclaration; }
        }

        public static ImageMoniker Include {
            get { return KnownMonikers.ClassFile; }
        }

        public static ImageMoniker GoToNodeDeclaration {
            get { return KnownMonikers.GoToReference; }
        }

        public static ImageMoniker GoToMethodPublic {
            get { return KnownMonikers.MethodPublic; }
        }

        public static ImageMoniker GoToClassPublic {
            get { return KnownMonikers.ClassPublic; }
        }

        public static ImageMoniker GoToInterfacePublic {
            get { return KnownMonikers.InterfacePublic; }
        }

        #endregion

        #region Symbols

        static IImageHandle TaskDeclarationImageHandle;

        public static ImageMoniker TaskDeclaration {
            get {

                if (TaskDeclarationImageHandle == null) {

                    TaskDeclarationImageHandle = GetCompositedImageHandle(
                        CreateLayer(TaskDefinition),
                        CreateLayer(KnownMonikers.ReferencedElement));
                }

                return TaskDeclarationImageHandle.Moniker;
            }
        }

        public static ImageMoniker InitConnectionPoint {
            get { return KnownMonikers.InputPin; }
        }

        public static ImageMoniker ExitConnectionPoint {
            get { return KnownMonikers.OutputPin; }
        }

        public static ImageMoniker EndConnectionPoint {
            get { return KnownMonikers.ActivityFinalNode; }
        }

        public static ImageMoniker TaskDefinition {
            get { return KnownMonikers.ActivityDiagram; }
        }

        public static ImageMoniker InitNode {
            get { return KnownMonikers.InputPin; }
        }

        public static ImageMoniker ExitNode {
            get { return KnownMonikers.OutputPin; }
        }

        public static ImageMoniker EndNode {
            get { return KnownMonikers.ActivityFinalNode; }
        }

        public static ImageMoniker TaskNode {
            get { return KnownMonikers.ActivityDiagram; }
        }

        public static ImageMoniker ChoiceNode {
            get { return KnownMonikers.DecisionNode; }
        }

        public static ImageMoniker ViewNode {
            get { return KnownMonikers.WindowsForm; }
        }

        public static ImageMoniker DialogNode {
            get { return KnownMonikers.Dialog; }
        }

        public static ImageMoniker SignalTrigger {
            get { return KnownMonikers.EventTrigger; }
        }

        #endregion

        #region Helper

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

        static IImageHandle GetCompositedImageHandle(params ImageCompositionLayer[] layers) {

            var imageService = NavLanguagePackage.GetGlobalService< SVsImageService, IVsImageService2>();
            
            var imageHandle = imageService.AddCustomCompositeImage(
                virtualWidth : 16,
                virtualHeight: 16,
                layerCount   : layers.Length,
                layers       : layers);


            return imageHandle;
        }

        #endregion
    }
}