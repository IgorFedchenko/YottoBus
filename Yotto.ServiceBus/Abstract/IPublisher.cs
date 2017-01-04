using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    interface IPublisher
    {
        void Start(int bindPort, PeerIdentity selfIdentity);

        void Start(IPEndPoint subscriber, PeerIdentity selfIdentity);

        void Start(List<IPEndPoint> subscribers, PeerIdentity selfIdentity);

        void Stop();

        void Publish(object message);

        void Send(object message, PeerIdentity peer);
    }
}
