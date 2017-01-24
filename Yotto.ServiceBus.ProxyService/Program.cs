using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yotto.ServiceBus.Proxy.Concrete;
using Yotto.ServiceBus.Proxy.Configuration;

namespace Yotto.ServiceBus.ProxyService
{
    /// <summary>
    /// This is a simple host for proxy <see cref="YottoBusProxy"/>
    /// </summary>
    class Program
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "proxy.config");

        static void Main(string[] args)
        {
            var proxyConfiguration = LoadConfiguration();

            using (var proxy = new YottoBusProxy())
            {
                proxy.Start(proxyConfiguration);

                Console.ReadLine();
            }
        }

        static ProxyConfiguration LoadConfiguration()
        {
            if (!File.Exists(ConfigPath))
            {
                var config = new ProxyConfiguration()
                {
                    BusPublisherPort = 19800,
                    DiscoveryEndpointPatterns = new List<string>()
                    {
                        "127.0.0.1"
                    },
                    PortForPublishers = 19876,
                    PortForSubscribers = 19877
                };

                new FileInfo(ConfigPath).Directory.Create();
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(config));
            }

            var configText = File.ReadAllText(ConfigPath);
            return JsonConvert.DeserializeObject<ProxyConfiguration>(configText);
        }
    }
}
