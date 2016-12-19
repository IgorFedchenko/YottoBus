using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Model
{
    public class PeerIdentity
    {
        internal PeerIdentity(IPEndPoint endpoint, TagsList tags)
        {
            Id = Guid.NewGuid();
            Endpoint = endpoint;
            Tags = tags;
        }

        public Guid Id { get; }
        public IPEndPoint Endpoint { get; }
        public TagsList Tags { get; }
    }
}
