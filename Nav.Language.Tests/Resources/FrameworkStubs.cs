using Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.WFL;
using Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.IWFL;
using Pharmatechnik.Apotheke.XTplus.Framework.Core.IWFL;
// ReSharper disable InconsistentNaming

namespace Pharmatechnik.Apotheke.XTplus.Framework.Core.IWFL {
    public interface IWFServiceBase : IWFService {
        INavCommand EscapeTask(TO to);
    }
}

namespace Pharmatechnik.Apotheke.XTplus.Framework.Core.WFL {

    public abstract class StandardWFS : BaseWFService, IWFServiceBase {
        public virtual INavCommand EscapeTask(TO to) {
            return null;
        }
    }

    public abstract class StandardWFS<TState> : BaseWFService<TState>, IWFServiceBase {
        public virtual INavCommand EscapeTask(TO to) {
            return null;
        }
    }
}

namespace Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.IWFL {
    public interface INavCommand {
    }

    public interface IINIT_TASK : INavCommand {
    }

    public interface CANCEL: IINIT_TASK { }

    public interface TASK_RESULT : INavCommand { }

    public delegate IINIT_TASK BeginTaskWrapper();

    public interface INavCommandBody {
    }

    public interface TO : INavCommandBody {
    }

    public interface IWFService {
    }
    public interface IClientSideWFS {
    }

}

namespace Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.WFL {
    public interface IBeginWFService {
    }

    public abstract class BaseWFService : IWFService {

        public INavCommandBody InternalTaskResult<TResult>(TResult result) {
            return null;
        }
    }

    // ReSharper disable once UnusedTypeParameter
    public abstract class BaseWFService<TState> : BaseWFService {

    }
}

namespace Pharmatechnik.Apotheke.XTplus.Common.IWFL {
    public interface ILegacyMessageBoxWFS : IWFServiceBase, IWFService {
        INavCommand End(TO to);
    }
}
