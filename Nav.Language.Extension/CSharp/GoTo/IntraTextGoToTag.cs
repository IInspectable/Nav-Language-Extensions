#region Using Directives

using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.Extension.GoToLocation;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class IntraTextGoToTag : GoToTag {

        public IntraTextGoToTag(ILocationInfoProvider provider, NavTaskAnnotation navTaskAnnotation) 
            : base(provider) {
            TaskAnnotation = navTaskAnnotation;
        }

        public NavTaskAnnotation TaskAnnotation { get; }

        public ImageMoniker ImageMoniker {
            get;set;
        }

        public object ToolTip {
            get; set;
        }
    }
}