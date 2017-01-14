﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Model.Messages
{
    /// <summary>
    /// This message delivered to local subscribers when any peer was lost on the bus
    /// </summary>
    public class PeerDisconnected
    {
        public PeerDisconnected(PeerIdentity identity)
        {
            Identity = identity;
        }

        public PeerIdentity Identity { get; }
    }
}
