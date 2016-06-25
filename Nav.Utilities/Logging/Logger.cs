#region Using Directives

using System.ComponentModel;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Utilities.Logging {

    public class Logger {

        readonly NLog.Logger _loggerImpl;

        Logger(NLog.Logger loggerImpl) {
            _loggerImpl = loggerImpl;

        }
        
        public static Logger Create<T>() {
            NLog.Logger baseLogger= NLog.LogManager.GetLogger(typeof(T).FullName);
            return new Logger(baseLogger);
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Debug([Localizable(false)] string message) {
            _loggerImpl.Debug(message);
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        [StringFormatMethod("message")]
        public void Debug([Localizable(false)] string message, params object[] args) {
            _loggerImpl.Debug(message, args);
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Info([Localizable(false)] string message) {
            _loggerImpl.Info(message);
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        [StringFormatMethod("message")]
        public void Info([Localizable(false)] string message, params object[] args) {
            _loggerImpl.Info(message, args);
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Warn([Localizable(false)] string message) {
            _loggerImpl.Warn(message);
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        [StringFormatMethod("message")]
        public void Warn([Localizable(false)] string message, params object[] args) {
            _loggerImpl.Debug(message, args);
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Error([Localizable(false)] string message) {
            _loggerImpl.Error(message);
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        [StringFormatMethod("message")]
        public void Error([Localizable(false)] string message, params object[] args) {
            _loggerImpl.Error(message, args);
        }
    }
}
