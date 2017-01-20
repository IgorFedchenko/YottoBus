using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    /// <summary>
    /// Non-generic interface. Use generic version to register your handlers.
    /// </summary>
    public interface IMessageHandler { }

    /// <summary>
    /// Marks implementing class as capable to handle particular message type.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to handle.</typeparam>
    public interface IMessageHandler<in TMessage> : IMessageHandler
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender of the message.</param>
        void Handle(TMessage message, PeerIdentity sender);
    }
}
