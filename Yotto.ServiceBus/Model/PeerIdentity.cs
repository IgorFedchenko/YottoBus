using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yotto.ServiceBus.Model
{
    public class PeerIdentity
    {
        public PeerIdentity() { }

        public PeerIdentity(PeerMetadata metadata)
        {
            Id = Guid.NewGuid();
            Metadata = metadata;
        }

        public Guid Id { get; set; }
        public PeerMetadata Metadata { get; set; }

        public override bool Equals(object obj)
        {
            var identity = obj as PeerIdentity;
            if (identity != null)
            {
                return Id == identity.Id;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
