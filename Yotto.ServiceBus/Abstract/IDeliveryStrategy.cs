using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    /// <summary>
    /// Describes object, capable to deliver message to subscribers accoring to some strategy
    /// </summary>
    public interface IDeliveryStrategy
    {
        /// <summary>
        /// Delivers the message to specified subscribers.
        /// </summary>
        /// <param name="message">The message to delivered.</param>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="subscribers">The subscribers to deliver.</param>
        void DeliverMessage(object message, PeerIdentity sender, IEnumerable<IMessageHandler> subscribers);
    }
}
