#region Using Directives

using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Text.Adornments;

using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion {

    static class CompletionImages {

        public static ImageElement Keyword      = new ImageElement(ImageMonikers.Keyword.ToImageId());
        public static ImageElement Folder       = new ImageElement(ImageMonikers.FolderClosed.ToImageId());
        public static ImageElement NavFile      = new ImageElement(ImageMonikers.Include.ToImageId());
        public static ImageElement File         = new ImageElement(ImageMonikers.File.ToImageId());
        public static ImageElement ParentFolder = new ImageElement(ImageMonikers.ParentFolder.ToImageId());

        public static ImageElement Choice          = new ImageElement(ImageMonikers.ChoiceNode.ToImageId());
        public static ImageElement Task            = new ImageElement(ImageMonikers.TaskNode.ToImageId());
        public static ImageElement GuiNode         = new ImageElement(ImageMonikers.ViewNode.ToImageId());
        public static ImageElement ConnectionPoint = new ImageElement(ImageMonikers.ExitConnectionPoint.ToImageId());

        public static ImageElement FromSymbol(ISymbol symbol) => new ImageElement(ImageMonikers.FromSymbol(symbol).ToImageId());
    }

}