using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Model.Messages
{
    /// <summary>
    /// This message delivered to local subscribers when any new peer was found on the bus
    /// </summary>
    public class PeerConnected
    {
        public PeerConnected(PeerIdentity identity)
        {
            Identity = identity;
        }

        /// <summary>
        /// Identity of the connected peer.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        public PeerIdentity Identity { get; }
    }
}
