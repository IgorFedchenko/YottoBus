using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;
using Yotto.ServiceBus.Model.Messages;

namespace Yotto.ServiceBus.Abstract
{
    interface ISubscriber
    {
        event Action<Message> MessageReceived; 

        void SubscribeTo<TMessage>();

        void UnsubscribeFrom<TMessage>();

        void Start(int bindPort, PeerIdentity peer);
        void Start(IPEndPoint publisher, PeerIdentity peer);
        void Start(List<IPEndPoint> publishers, PeerIdentity peer);

        void Stop();
    }
}
