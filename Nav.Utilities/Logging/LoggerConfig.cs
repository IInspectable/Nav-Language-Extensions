#region Using Directives

using System.IO;

using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

#endregion
namespace Pharmatechnik.Nav.Utilities.Logging {

    public static class LoggerConfig {

        // ReSharper disable InconsistentNaming
        const long KB = 1024;
        const long MB = 1024 * KB;
        // ReSharper restore InconsistentNaming

        public static void Initialize(string logFolder, string logName) {
            
            LoggingConfiguration loggingConfiguration = new LoggingConfiguration();
            var fileTarget = new FileTarget {
                FileName         = Path.Combine(logFolder, $"{logName}.log.xml"),
                ArchiveFileName  = Path.Combine(logFolder, $"{logName}.log.xml.{{#####}}"),
                ArchiveNumbering = ArchiveNumberingMode.Rolling,
                MaxArchiveFiles  = 10,
                ArchiveAboveSize = 1 * MB,
                Layout           = new Log4JXmlEventLayout()
            };

            loggingConfiguration.AddTarget("file", fileTarget);
            loggingConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));

            LogManager.Configuration = loggingConfiguration;
        }
    }
}