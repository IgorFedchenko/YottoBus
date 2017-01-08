using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetMQ;

namespace Yotto.ServiceBus.Proxy.Concrete
{
    /// <summary>
    /// Manages subscriptions of the given NetMQ Socket according to given endpoints list
    /// </summary>
    class DynamicSubscriber
    {
        private readonly NetMQSocket _subscriberSocket;
        private readonly List<IPEndPoint> _endpointsForDiscovering;

        public DynamicSubscriber(NetMQSocket subscriberSocket, List<IPEndPoint> endpointsForDiscovering)
        {
            _subscriberSocket = subscriberSocket;
            _endpointsForDiscovering = endpointsForDiscovering;
        }

        public void StartDiscovering()
        {
            foreach (var endpoint in _endpointsForDiscovering)
            {
                CheckEndpoint(endpoint);
            }
        }

        private void CheckEndpoint(IPEndPoint endpoint)
        {
            _subscriberSocket.Connect($"tcp://{endpoint.Address}:{endpoint.Port}");
        }
    }
}
