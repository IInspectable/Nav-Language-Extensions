#region Using Directives

using System;
using System.Diagnostics;
using System.ComponentModel;

#endregion

namespace Pharmatechnik.Nav.Utilities.Logging {

    public class Logger {

        readonly NLog.Logger _loggerImpl;

        [ThreadStatic]
        static int IndentLevel;   
        const int IndentSize = 3;

        protected Logger(NLog.Logger loggerImpl) {
            _loggerImpl  = loggerImpl;
        }
        
        public static Logger Create<T>() {
            NLog.Logger baseLogger= NLog.LogManager.GetLogger(typeof(T).FullName);
            return new Logger(baseLogger);
        }

        public IDisposable LogBlock(string blockName) {
            return new BlockLogger(this, blockName);
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Trace([Localizable(false)] string message) {
            _loggerImpl.Trace(IndentMessage(message));
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Debug([Localizable(false)] string message) {
            _loggerImpl.Debug(IndentMessage(message));
        }
        
        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Info([Localizable(false)] string message) {
            _loggerImpl.Info(IndentMessage(message));
        }
        
        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Warn([Localizable(false)] string message) {
            _loggerImpl.Warn(IndentMessage(message));
        }
       
        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Error([Localizable(false)] string message) {
            _loggerImpl.Error(IndentMessage(message));
        }

        string IndentMessage(string message) {
            return new string(' ', IndentLevel * IndentSize) + message;
        }

        sealed class BlockLogger : IDisposable {

            readonly Logger _logger;
            readonly string _blockName;
            readonly Stopwatch _stopwatch;

            public BlockLogger(Logger logger, string blockName) {

                _stopwatch = Stopwatch.StartNew();
                _logger    = logger;
                _blockName = blockName;
                
                _logger.Trace($"Begin {_blockName}");

                IndentLevel++;
            }

            public void Dispose() {

                IndentLevel = Math.Max(0, IndentLevel - 1);

                _logger.Trace($"End {_blockName} elapsed time: {_stopwatch.Elapsed}");
            }
        }
    }    
}