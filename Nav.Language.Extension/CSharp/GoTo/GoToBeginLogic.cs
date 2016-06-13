#region Using Directives

using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoTo {

    class GoToBeginLogic: IntraTextGoToTag {

        //public GoToBeginLogic(string taskName) {
        //    
        //}

        public override Task<Location> GoToLocationAsync(CancellationToken cancellationToken = new CancellationToken()) {
            // TODO Implement GoToLocationAsync
            return Task.FromResult< Location>(null);
        }

        public override ImageMoniker ImageMoniker {
            get { return SymbolImageMonikers.InitNode; }
        }

        public override object ToolTip {
            get { return ""; }
        }

    }

}