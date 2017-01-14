using System;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Loggers.Console
{
    public class ConsoleLogger : IBusLogger
    {
        public LogLevel LogLevel { get; private set; }

        public void SetLogLevel(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public void Trace(string message)
        {
            System.Console.WriteLine(">>> Trace: " + message);
        }

        public void Debug(string message)
        {
            System.Console.WriteLine(">>> Debug: " + message);
        }

        public void Info(string message)
        {
            System.Console.WriteLine(">>> Info: " + message);
        }

        public void Warning(string message)
        {
            System.Console.WriteLine(">>> Warning: " + message);
        }

        public void Error(string message, Exception ex = null)
        {
            System.Console.WriteLine(">>> Error: " + message + "\nException: " + ex);
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
    }
}
