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
    class HostService
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "proxy.config");
        private YottoBusProxy _proxy;

        public bool Start(IDictionary<string, object> parameters)
        {
            var proxyConfiguration = LoadConfiguration();

            _proxy = new YottoBusProxy();

            _proxy.Start(proxyConfiguration);

            return true;
        }

        public void Stop()
        {
            _proxy?.Dispose();
        }

        private ProxyConfiguration LoadConfiguration()
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
