using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Proxy.Configuration
{
    /// <summary>
    /// Configuration for proxy
    /// </summary>
    public class ProxyConfiguration
    {
        /// <summary>
        /// Gets or sets the port for local subscribers.
        /// </summary>
        /// <value>
        /// The port for subscribers.
        /// </value>
        public int PortForSubscribers { get; set; }

        /// <summary>
        /// Gets or sets the port for local publishers.
        /// </summary>
        /// <value>
        /// The port for publishers.
        /// </value>
        public int PortForPublishers { get; set; }

        /// <summary>
        /// Gets or sets the bus publisher port (for other brokers).
        /// </summary>
        /// <value>
        /// The bus publisher port.
        /// </value>
        public int BusPublisherPort { get; set; }

        /// <summary>
        /// Gets or sets the discovery endpoint patterns list.
        /// </summary>
        /// <value>
        /// The discovery endpoint patterns.
        /// </value>
        public List<string> DiscoveryEndpointPatterns { get; set; }
    }
}
