using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Fclp;

namespace Nav.Language.Service {

    class CommandLineParamAttribute : Attribute {
    }

    public class ServiceConnectionArgs {

        public ServiceConnectionArgs() {
            HostUri = new Uri("net.pipe://localhost/");
            EndpointId = Guid.Empty.ToString();
        }

        [CommandLineParam]
        public string EndpointId { get; private set; }

        [CommandLineParam]
        public int OwnerProcessId { get; private set; }

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

        public const long BindingMaxReceivedMessageSize = 10000000;

        public static ServiceConnectionArgs CreateNew() {
            var sca = new ServiceConnectionArgs {
                OwnerProcessId = Process.GetCurrentProcess().Id,
                EndpointId = Guid.NewGuid().ToString(),
            };

            return sca;
        }

        public static ServiceConnectionArgs FromCommandLine(string[] commandline) {

            var clp = new FluentCommandLineParser<ServiceConnectionArgs>();
            clp.Setup(i => i.OwnerProcessId).As(nameof(OwnerProcessId)).Required();
            clp.Setup(i => i.EndpointId).As(nameof(EndpointId)).Required();

            var result = clp.Parse(commandline);
            if (result.HasErrors) {
                Console.WriteLine(result.ErrorText);
                return null;
            }

            ServiceConnectionArgs cla = clp.Object;

            return cla;
        }

        public string ToCommandLine() {

            return string.Join(" ", GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<CommandLineParamAttribute>() != null)
                .Select(property => {

                    var name = property.Name;
                    var value = property.GetValue(this).ToString();
                    var arg = $"--{name} {value}";
                    return arg;
                }));
        }
    }
}