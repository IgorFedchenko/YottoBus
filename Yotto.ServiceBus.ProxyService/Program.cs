using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Topshelf;
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
            if (args.Contains("install") || args.Contains("uninstall") || Environment.OSVersion.Platform != PlatformID.Unix)
            {
                HostFactory.Run(config => {
                    var parameters = config.SelectPlatform();

                    config.Service<HostService>(s => {
                        s.ConstructUsing(name => new HostService());
                        s.WhenStarted(host => host.Start(parameters));
                        s.WhenStopped(host => host.Stop());
                    });
                    
                    config.SetServiceName("YottoBus.ProxyHost");
                    config.SetDisplayName("YottoBus.ProxyHost");
                    config.SetDescription("YottoBus.ProxyHost");
                    config.StartAutomatically();
                });
            }
            else
            {
                var service = new HostService();
                service.Start(null);
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
