using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Proxy.Concrete;
using Yotto.ServiceBus.Proxy.Configuration;

namespace Yotto.ServiceBus.ProxyService
{
    /// <summary>
    /// This is a simple host for proxy <see cref="YottoBusProxy"/>
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Parse proxy config from file
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

            using (var proxy = new YottoBusProxy())
            {
                proxy.Start(proxyConfiguration);

                Console.ReadLine();
            }
        }
    }
}
