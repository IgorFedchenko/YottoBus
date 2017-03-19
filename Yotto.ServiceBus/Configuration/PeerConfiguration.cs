using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Configuration
{
    /// <summary>
    /// Describes peer configuration used for peer creation
    /// </summary>
    public class PeerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PeerConfiguration"/> class.
        /// </summary>
        /// <param name="peerContextName">Context name to create peer</param>
        /// <param name="attributes">The attributes of the peer.</param>
        public PeerConfiguration(string peerContextName, Dictionary<string, string> attributes = null)
        {
            Context = peerContextName;
            Metadata = new PeerMetadata(attributes ?? new Dictionary<string, string>());
        }

        /// <summary>
        /// Context in which peer will be created
        /// </summary>
        public string Context { get; }

        /// <summary>
        /// Gets or sets the peer metadata. 
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public PeerMetadata Metadata { get; }
    }
}
