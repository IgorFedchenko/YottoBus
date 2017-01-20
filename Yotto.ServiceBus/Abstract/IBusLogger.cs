using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    /// <summary>
    /// Describes interface of logger, which may be used to log bus events
    /// </summary>
    public interface IBusLogger
    {
        /// <summary>
        /// Current logging level
        /// </summary>
        LogLevel LogLevel { get; }

        /// <summary>
        /// Sets logging level
        /// </summary>
        /// <param name="logLevel">Level to set</param>
        void SetLogLevel(LogLevel logLevel);

        /// <summary>
        /// Logs message on trace level
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Trace(string message);

        /// <summary>
        /// Logs message on debug level
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Debug(string message);

        /// <summary>
        /// Logs message on info level
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Info(string message);

        /// <summary>
        /// Logs message on warning level
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Warning(string message);

        /// <summary>
        /// Logs message on error level
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Error(string message, Exception ex = null);

        /// <summary>
        /// Logs message on specified level
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to log.</param>
        void Log(LogLevel level, string message);
    }
}
