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
using Yotto.ServiceBus.Proxy.Model;

namespace Yotto.ServiceBus.Proxy.Concrete
{
    /// <summary>
    /// Represents bus broker class. Transfers messages back and forth.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class YottoBusProxy : IDisposable
    {
        private NetMQ.Proxy _externalToLocalProxy;
        private NetMQ.Proxy _localToExternalProxy;

        /// <summary>
        /// Starts the proxy with specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void Start(ProxyConfiguration configuration)
        {
            var endpointPatterns = configuration.DiscoveryEndpointPatterns;
            var discoveryEndpoints = endpointPatterns.Select(pattern => new EndpointsRange(pattern)).Aggregate((range1, range2) => range1.JoinWith(range2));
            StartExternalToLocalTransfer(configuration.PortForSubscribers, discoveryEndpoints);
            StartLocalToExternalTransfer(configuration.PortForPublishers, configuration.BusPublisherPort);
        }

        /// <summary>
        /// Stops this proxy.
        /// </summary>
        public void Stop()
        {
            _localToExternalProxy?.Stop();
            _externalToLocalProxy?.Stop();
        }

        /// <summary>
        /// Stops this proxy.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        /// Starts the external to local message transfering.
        /// </summary>
        /// <param name="portForSubscribers">The port for local subscribers.</param>
        /// <param name="discoveryEndpoints">The endpoints to perform other brokers discovery.</param>
        private void StartExternalToLocalTransfer(int portForSubscribers, EndpointsRange discoveryEndpoints)
        {
            Task.Run(() =>
            {
                using (var xsubSocket = new XSubscriberSocket())
                using (var xpubSocket = new XPublisherSocket())
                {
                    xpubSocket.Bind($"tcp://localhost:{portForSubscribers}");

                    var dynamicSubscriber = new DynamicSubscriber(xsubSocket, discoveryEndpoints);
                    dynamicSubscriber.StartDiscovering();

                    _externalToLocalProxy = new NetMQ.Proxy(xpubSocket, xsubSocket);

                    _externalToLocalProxy.Start();
                }
            });
        }

        /// <summary>
        /// Starts the local to external message transfering.
        /// </summary>
        /// <param name="proxyForPublishersPort">The port for local publishers.</param>
        /// <param name="busPublisherPort">Broker's publisher port for other brokers subscription.</param>
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

                    _localToExternalProxy = new NetMQ.Proxy(subSocket, pubSocket);

                    _localToExternalProxy.Start();
                }
            });
        }
    }
}
