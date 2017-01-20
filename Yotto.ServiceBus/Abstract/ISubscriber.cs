using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;
using Yotto.ServiceBus.Model.Messages;

namespace Yotto.ServiceBus.Abstract
{
    /// <summary>
    /// Describes subscriber, capable to get messages from the bus
    /// </summary>
    interface ISubscriber
    {
        /// <summary>
        /// Occurs when new message received.
        /// </summary>
        event Action<Message> MessageReceived;

        /// <summary>
        /// Add subscrubtion to given message type
        /// </summary>
        /// <param name="messageType">Type of the message to subscrube.</param>
        void SubscribeTo(Type messageType);

        /// <summary>
        /// Unsubscribes from specified message type.
        /// </summary>
        /// <param name="messageType">Type of the message to unsubscrube.</param>
        void UnsubscribeFrom(Type messageType);

        /// <summary>
        /// Starts this instance, connecting it to specified publisher.
        /// </summary>
        /// <param name="publisher">The publisher to connect.</param>
        /// <param name="peer">The self identity.</param>
        void Start(IPEndPoint publisher, PeerIdentity peer);

        /// <summary>
        /// Starts this instance, connecting it to specified publishers.
        /// </summary>
        /// <param name="publishers">The publishers to connect.</param>
        /// <param name="peer">The self peer identoty.</param>
        void Start(List<IPEndPoint> publishers, PeerIdentity peer);

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
    }
}
