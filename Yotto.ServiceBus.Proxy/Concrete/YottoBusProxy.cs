using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Yotto.ServiceBus.Proxy.Configuration;
using Yotto.ServiceBus.Proxy.Helpers;

namespace Yotto.ServiceBus.Proxy.Concrete
{
    public class YottoBusProxy
    {
        public void Start(ProxyConfiguration configuration)
        {
            var endpointPatterns = configuration.DiscoveryAddressPatterns.Select(pattern => pattern + ":" + configuration.BusPublisherPort);
            var discoveryEndpoints = endpointPatterns.Select(EndpointsRangeParser.Parse).Aggregate((endpoints1, endpoints2) => endpoints1.Concat(endpoints2).ToList()).ToList();
            StartExternalToLocalTransfer(configuration.PortForSubscribers, discoveryEndpoints);
            StartLocalToExternalTransfer(configuration.PortForPublishers, configuration.BusPublisherPort);
        }

        private void StartExternalToLocalTransfer(int portForSubscribers, List<IPEndPoint> discoveryEndpoints)
        {
            Task.Run(() =>
            {
                using (var xsubSocket = new XSubscriberSocket())
                using (var xpubSocket = new XPublisherSocket())
                {
                    xpubSocket.Bind($"tcp://localhost:{portForSubscribers}");

                    var dynamicSubscriber = new DynamicSubscriber(xsubSocket, discoveryEndpoints);
                    dynamicSubscriber.StartDiscovering();

                    var proxy = new NetMQ.Proxy(xpubSocket, xsubSocket);

                    proxy.Start();
                }
            });
        }

        private void StartLocalToExternalTransfer(int proxyForPublishersPort, int busPublisherPort)
        {
            Task.Run(() =>
            {
                using (var subSocket = new SubscriberSocket())
                using (var pubSocket = new PublisherSocket())
                {
                    subSocket.SubscribeToAnyTopic();
                    subSocket.Bind($"tcp://localhost:{proxyForPublishersPort}");

                    pubSocket.Bind($"tcp://localhost:{busPublisherPort}");

                    var proxy = new NetMQ.Proxy(subSocket, pubSocket);

                    proxy.Start();
                }
            });
        }
    }
}
