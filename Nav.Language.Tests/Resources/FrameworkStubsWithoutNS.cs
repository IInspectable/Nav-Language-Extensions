using Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.WFL;
using Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.IWFL;
using Pharmatechnik.Apotheke.XTplus.Framework.Core.IWFL;
// ReSharper disable InconsistentNaming

public interface IWFServiceBase : IWFService {
    INavCommand EscapeTask(TO to);
}


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

public interface INavCommand {
}

public interface IINIT_TASK : INavCommand {
}
public interface IINIT_TASK<T> : INavCommand {
}

public interface CANCEL : IINIT_TASK {
}

public interface TASK_RESULT : IINIT_TASK, INavCommand {
}

public delegate IINIT_TASK BeginTaskWrapper();
public delegate IINIT_TASK<TResult> BeginTaskWrapper<TResult>();
public delegate INavCommand AfterDelegate1<ResultType>(ResultType result);
public delegate INavCommand AfterDelegate2<ResultType, P1>(ResultType result, P1 p1);

public class GOTO_TASK : IINIT_TASK { }

public class GOTO_GUI : IINIT_TASK {

}

public class OPEN_MODAL_GUI : IINIT_TASK {
}

public class START_NONMODAL_TASK : INavCommand {
}

public class START_MODAL_TASK : INavCommand {
}

public interface INavCommandBody {
}

public interface TO : INavCommandBody {
}

public interface IWFService {
}

public interface IClientSideWFS {
}


public interface IBeginWFService {
}

public abstract class BaseWFService : IWFService {

    public INavCommandBody InternalTaskResult<TResult>(TResult result) {
        return null;
    }

    public GOTO_GUI GotoGUI(TO to) {
        return null;
    }

    public OPEN_MODAL_GUI OpenModalGUI(TO to) {
        return null;
    }

    public START_NONMODAL_TASK StartNonModalGUI(TO to) {
        return null;
    }

    public START_MODAL_TASK OpenModalTask<TResult>(BeginTaskWrapper wrapped, AfterDelegate1<TResult> after) {
        return null;
    }


    public GOTO_TASK GotoTask<TResult>(BeginTaskWrapper wrapped, AfterDelegate1<TResult> after) {
        return null;
    }

    public GOTO_TASK GotoTask<TResult>(BeginTaskWrapper<TResult> wrapped, AfterDelegate1<TResult> after) {
        return null;
    }
}

// ReSharper disable once UnusedTypeParameter
public abstract class BaseWFService<TState> : BaseWFService {

}

public interface ILegacyMessageBoxWFS : IWFServiceBase, IWFService {
    INavCommand End(TO to);
}
