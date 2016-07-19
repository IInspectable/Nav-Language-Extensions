#region Using Directives

using System;

using Microsoft.VisualStudio.OLE.Interop;

using Pharmatechnik.Nav.Utilities.Logging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    static class ComEventSink {

        public static IDisposable Advise<T>(object obj, T sink) where T : class {

            if (!typeof(T).IsInterface) {
                throw new InvalidOperationException();
            }

            var connectionPointContainer = obj as IConnectionPointContainer;
            if (connectionPointContainer == null) {
                throw new ArgumentException("Not an IConnectionPointContainer", nameof(obj));
            }

            IConnectionPoint connectionPoint;
            connectionPointContainer.FindConnectionPoint(typeof(T).GUID, out connectionPoint);
            if (connectionPoint == null) {
                throw new InvalidOperationException("Could not find connection point for " + typeof(T).FullName);
            }

            uint cookie;
            connectionPoint.Advise(sink, out cookie);

            return new ComEventSinkImpl(connectionPoint, cookie);
        }

        sealed class ComEventSinkImpl : IDisposable {

            static readonly Logger Logger = Logger.Create(typeof(ComEventSinkImpl));

            readonly IConnectionPoint _connectionPoint;
            readonly uint _cookie;
            bool _unadvised;

            public ComEventSinkImpl(IConnectionPoint connectionPoint, uint cookie) {
                _connectionPoint = connectionPoint;
                _cookie          = cookie;
            }

            public void Dispose() {

                if (_unadvised) {
                    Logger.Error("Already unadvised.");
                    return;
                }

                _connectionPoint.Unadvise(_cookie);
                _unadvised = true;
            }
        }
    }   
}