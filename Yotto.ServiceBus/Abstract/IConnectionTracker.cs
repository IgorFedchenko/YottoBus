using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    /// <summary>
    /// Interface, used to keep track of other peers in network and handling heartbeats
    /// </summary>
    interface IConnectionTracker
    {
        /// <summary>
        /// Starts peers tracking.
        /// </summary>
        /// <param name="publisher">The publisher to use.</param>
        /// <param name="subscriber">The subscriber to use.</param>
        /// <param name="selfIdentity">The self identity of peer.</param>
        void Start(IPublisher publisher, ISubscriber subscriber, PeerIdentity selfIdentity);

        /// <summary>
        /// Stops tracking.
        /// </summary>
        void Stop();

        /// <summary>
        /// Occurs when new peer connected.
        /// </summary>
        event Action<PeerIdentity> PeerConnected;

        /// <summary>
        /// Occurs when new peer disconnected.
        /// </summary>
        event Action<PeerIdentity> PeerDisconnected;
    }
}
