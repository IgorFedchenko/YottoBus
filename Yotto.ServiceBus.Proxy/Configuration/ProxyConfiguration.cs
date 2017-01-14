using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Proxy.Configuration
{
    public class ProxyConfiguration
    {
        public int PortForSubscribers { get; set; }
        public int PortForPublishers { get; set; }
        public int BusPublisherPort { get; set; }
        public List<string> DiscoveryEndpointPatterns { get; set; }
    }
}
