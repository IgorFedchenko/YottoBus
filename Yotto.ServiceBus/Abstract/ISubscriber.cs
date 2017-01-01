﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    interface ISubscriber
    {
        event Action<PeerIdentity, object> MessageReceived;

        void SubscribeTo<TMessage>();

        void UnsubscribeFrom<TMessage>();

        void Start(int bindPort);
        void Start(IPEndPoint publisher);
        void Start(List<IPEndPoint> publishers);

        void Stop();
    }
}
