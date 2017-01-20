using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yotto.ServiceBus.Model
{
    /// <summary>
    /// Identity of the peer
    /// </summary>
    public class PeerIdentity
    {
        public PeerIdentity() { }

        public PeerIdentity(PeerMetadata metadata)
        {
            Id = Guid.NewGuid();
            Metadata = metadata;
        }

        /// <summary>
        /// Unique peer Id
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Peer metadata, as set of key-value pairs
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
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
