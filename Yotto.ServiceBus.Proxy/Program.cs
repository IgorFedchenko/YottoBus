using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Proxy.Concrete;
using Yotto.ServiceBus.Proxy.Configuration;

namespace Yotto.ServiceBus.Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxyConfiguration = new ProxyConfiguration()
            {
                BusPublisherPort = 19800,
                DiscoveryEndpointPatterns = new List<string>()
                {
                    "127.0.0.1:19800"
                },
                PortForPublishers = 19876,
                PortForSubscribers = 19877
            };

            var proxy = new YottoBusProxy();
            proxy.Start(proxyConfiguration);

            while (true)
            {
                
            }
        }
    }
}
