using System;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete.Loggers
{
    public class ConsoleLogger : IBusLogger
    {
        public LogLevel LogLevel { get; private set; } = LogLevel.Debug;

        public void SetLogLevel(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public void Trace(string message)
        {
            if (!LoggingEnabledFor(LogLevel.Trace))
                return;

            System.Console.WriteLine($">>> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Trace: " + message);
        }

        public void Debug(string message)
        {
            if (!LoggingEnabledFor(LogLevel.Debug))
                return;

            System.Console.WriteLine($">>> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Debug: " + message);
        }

        public void Info(string message)
        {
            if (!LoggingEnabledFor(LogLevel.Info))
                return;

            System.Console.WriteLine($">>> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Info: " + message);
        }

        public void Warning(string message)
        {
            if (!LoggingEnabledFor(LogLevel.Warning))
                return;

            System.Console.WriteLine($">>> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Warning: " + message);
        }

        public void Error(string message, Exception ex = null)
        {
            if (!LoggingEnabledFor(LogLevel.Error))
                return;

            System.Console.WriteLine($">>> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Error: " + message + "\nException: " + ex);
        }

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

        private bool LoggingEnabledFor(LogLevel logLevel)
        {
            return (int) LogLevel >= (int) logLevel;
        }
    }
}
