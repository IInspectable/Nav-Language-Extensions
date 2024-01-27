#region Using Directives
using System;
using Nav.Language.Tests.Regression.Test1.IWFL;
using Pharmatechnik.Apotheke.XTplus.Common.WFL;
using Pharmatechnik.Apotheke.XTplus.Common.IWFL;
using Pharmatechnik.Apotheke.XTplus.Framework.Core.WFL;
using Pharmatechnik.Apotheke.XTplus.Framework.Core.IWFL;
using Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.WFL;
using Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.IWFL;
#endregion

namespace Nav.Language.Tests.Regression.Test1.WFL {	
    public partial class ConcatSampleWFS {
        protected override INavCommandBody BeginLogic() {
            throw new NotImplementedException();
        }

        protected override INavCommandBody BeginLogic(string message,
                                                      Init2CallContext callContext) {
            throw new NotImplementedException();
        }

        protected override INavCommandBody AfterALogic(FooResult result) {
             throw new NotImplementedException();
        }

        protected override INavCommandBody AfterBLogic(FooResult result,
                                                       AfterBCallContext callContext) {
             throw new NotImplementedException();
        }

        protected override INavCommandBody AfterCLogic(FooResult result) {
             throw new NotImplementedException();
        }

        protected override INavCommandBody OnFooLogic(ViewTO to,
                                                      OnFooCallContext callContext) {
            throw new NotImplementedException();
        }
    }
}