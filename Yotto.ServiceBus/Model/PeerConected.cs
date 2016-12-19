﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Model
{
    public class PeerConected
    {
        public PeerConected(PeerIdentity peer)
        {
            Peer = peer;
        }

        public PeerIdentity Peer { get; }
    }
}