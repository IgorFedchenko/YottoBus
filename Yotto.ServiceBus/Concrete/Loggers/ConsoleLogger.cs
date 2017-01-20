using System;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete.Loggers
{
    /// <summary>
    /// This logger logs all messages to standart output
    /// </summary>
    /// <seealso cref="Yotto.ServiceBus.Abstract.IBusLogger" />
    public class ConsoleLogger : IBusLogger
    {
        /// <summary>
        /// Current logging level
        /// </summary>
        public LogLevel LogLevel { get; private set; } = LogLevel.Debug;

        /// <summary>
        /// Sets logging level
        /// </summary>
        /// <param name="logLevel">Level to set</param>
        public void SetLogLevel(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        /// <summary>
        /// Logs message on trace level
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Trace(string message)
        {
            if (!LoggingEnabledFor(LogLevel.Trace))
                return;

            System.Console.WriteLine($">>> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Trace: " + message);
        }

        /// <summary>
        /// Logs message on debug level
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Debug(string message)
        {
            if (!LoggingEnabledFor(LogLevel.Debug))
                return;

            System.Console.WriteLine($">>> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Debug: " + message);
        }

        /// <summary>
        /// Logs message on info level
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Info(string message)
        {
            if (!LoggingEnabledFor(LogLevel.Info))
                return;

            System.Console.WriteLine($">>> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Info: " + message);
        }

        /// <summary>
        /// Logs message on warning level
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Warning(string message)
        {
            if (!LoggingEnabledFor(LogLevel.Warning))
                return;

            System.Console.WriteLine($">>> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Warning: " + message);
        }

        /// <summary>
        /// Logs message on error level
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex"></param>
        public void Error(string message, Exception ex = null)
        {
            if (!LoggingEnabledFor(LogLevel.Error))
                return;

            System.Console.WriteLine($">>> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Error: " + message + "\nException: " + ex);
        }

        /// <summary>
        /// Logs message on specified level
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to log.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">level - null</exception>
        public void Log(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    Trace(message);
                    break;
                case LogLevel.Debug:
                    Debug(message);
                    break;
                case LogLevel.Info:
                    Info(message);
                    break;
                case LogLevel.Warning:
                    Warning(message);
                    break;
                case LogLevel.Error:
                    Error(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        /// <summary>
        /// Checks if logging is enables for specified level
        /// </summary>
        /// <param name="logLevel">The log level to check.</param>
        /// <returns><c>true</c>, if enabled, otherwise <c>false</c></returns>
        private bool LoggingEnabledFor(LogLevel logLevel)
        {
            return (int) LogLevel >= (int) logLevel;
        }
    }
}
