using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    public interface IBusLogger
    {
        LogLevel LogLevel { get; }

        void SetLogLevel(LogLevel logLevel);

        void Trace(string message);
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message, Exception ex = null);
        void Log(LogLevel level, string message);
    }
}
