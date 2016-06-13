using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.Extension.Common;

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    abstract class IntraTextGoToTag: GoToTag {
     
        public abstract ImageMoniker ImageMoniker { get; }
        public abstract object ToolTip { get; }
    }
}