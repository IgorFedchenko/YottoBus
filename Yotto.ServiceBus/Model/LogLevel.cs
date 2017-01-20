using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Model
{
    /// <summary>
    /// Log levels
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Log all messages
        /// </summary>
        Trace,
        /// <summary>
        /// The log all up to debug information
        /// </summary>
        Debug,
        /// <summary>
        /// The log all up to general information
        /// </summary>
        Info,
        /// <summary>
        /// Log only warnings and errors
        /// </summary>
        Warning,
        /// <summary>
        /// Log errors only
        /// </summary>
        Error
    }
}
