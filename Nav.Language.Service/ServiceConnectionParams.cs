#region Using Directives

using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

using Fclp;

using Pharmatechnik.Nav.Utilities.Logging;

#endregion

namespace Nav.Language.Service {

    class CommandLineParamAttribute : Attribute {
    }

    public class ServiceConnectionParams {

        static readonly Logger Logger = Logger.Create<ServiceConnectionParams>();

        public ServiceConnectionParams() {
            HostUri    = new Uri("net.pipe://localhost/");
            EndpointId = Guid.Empty.ToString();
        }

        [CommandLineParam]
        public string EndpointId { get; private set; }

        [CommandLineParam]
        public int ParentProcessId { get; private set; }

        public string ReadyEventName {
            get { return EndpointId; }
        }

        Uri HostUri { get; }

        public Uri BaseUri {
            get { return new Uri(HostUri, EndpointId); }
        }

        public string AutoCompletionAddress {
            get { return "AutoCompletionSource"; }
        }

        public Uri AutoCompletionUri {
            get { return new Uri(HostUri, EndpointId + "/" + AutoCompletionAddress); }
        }

        public long BindingMaxReceivedMessageSize {
            get { return 10000000; }
        }

        public static ServiceConnectionParams CreateNew() {
            var sca = new ServiceConnectionParams {
                ParentProcessId = Process.GetCurrentProcess().Id,
                EndpointId = Guid.NewGuid().ToString(),
            };

            return sca;
        }

        public static ServiceConnectionParams FromCommandLine(string[] commandline) {

            var clp = new FluentCommandLineParser<ServiceConnectionParams>();
            clp.Setup(i => i.ParentProcessId).As(nameof(ParentProcessId)).Required();
            clp.Setup(i => i.EndpointId).As(nameof(EndpointId)).Required();

            var result = clp.Parse(commandline);
            if (result.HasErrors) {
                Logger.Error($"Unable to parse command line:\n{result.ErrorText}");
                return null;
            }

            ServiceConnectionParams cla = clp.Object;

            return cla;
        }

        public string ToCommandLine() {

            return string.Join(" ", GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<CommandLineParamAttribute>() != null)
                .Select(property => {
                    var name  = property.Name;
                    var value = property.GetValue(this).ToString();
                    var arg   = $"--{name} {value}";
                    return arg;
                }));
        }
    }
}