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
        /// Gets or sets the peer metadata. 
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public PeerMetadata Metadata { get; set; }
    }
}
