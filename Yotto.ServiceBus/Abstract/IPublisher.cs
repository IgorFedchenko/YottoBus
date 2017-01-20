using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    /// <summary>
    /// Describes publisher, capable to put messages to the bus
    /// </summary>
    interface IPublisher
    {
        /// <summary>
        /// Starts the publisher, connecting it to specified subscruber.
        /// </summary>
        /// <param name="subscriber">The subscriber to connect.</param>
        /// <param name="selfIdentity">The self identity.</param>
        void Start(IPEndPoint subscriber, PeerIdentity selfIdentity);

        /// <summary>
        /// Starts the publisher, connecting it to specified subscrubers.
        /// </summary>
        /// <param name="subscribers">The subscribers to connect.</param>
        /// <param name="selfIdentity">The self identity.</param>
        void Start(List<IPEndPoint> subscribers, PeerIdentity selfIdentity);

        /// <summary>
        /// Stops this publisher.
        /// </summary>
        void Stop();

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        void Publish(object message);

        /// <summary>
        /// Sends the specified message to specified peer.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="peer">The peer to send to</param>
        void Send(object message, PeerIdentity peer);
    }
}
