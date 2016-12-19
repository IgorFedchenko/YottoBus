using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Akka.Event;

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
    }
}
