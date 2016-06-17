#region Using Directives

using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.Extension.GoToLocation;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class IntraTextGoToTag: GoToTag {

        public IntraTextGoToTag(ILocationInfoProvider provider, ImageMoniker imageMoniker, object toolTip) 
                : base(provider) {
            ImageMoniker = imageMoniker;
            ToolTip      = toolTip;
        }

        protected IntraTextGoToTag(ILocationInfoProvider provider) : base(provider) { 
        }
     
        public virtual ImageMoniker ImageMoniker { get; }
        public virtual object ToolTip { get; }
    }
}