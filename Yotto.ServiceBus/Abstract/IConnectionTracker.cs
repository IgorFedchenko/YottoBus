using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    interface IConnectionTracker
    {
        void Start(IPublisher publisher, ISubscriber subscriber, PeerIdentity selfIdentity);

        void Stop();

        event Action<PeerIdentity> PeerConnected;
        event Action<PeerIdentity> PeerDisconnected;
    }
}
