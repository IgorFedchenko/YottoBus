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
        public PeerIdentity() { }

        public PeerIdentity(IPEndPoint endpoint, TagsList tags)
        {
            Id = Guid.NewGuid();
            Endpoint = endpoint.Address + ":" + endpoint.Port;
            Tags = tags;
        }

        public Guid Id { get; set; }
        public string Endpoint { get; set; }
        public TagsList Tags { get; set; }

        public override bool Equals(object obj)
        {
            var identity = obj as PeerIdentity;
            if (identity != null)
            {
                return Id == identity.Id;
            }

            return base.Equals(obj);
        }
    }
}
