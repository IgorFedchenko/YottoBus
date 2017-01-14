using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yotto.ServiceBus.Model
{
    public class PeerMetadata
    {
        [JsonProperty]
        private readonly Dictionary<string, string> _metadata;

        public PeerMetadata() : this(new Dictionary<string, string>()) { }

        public PeerMetadata(Dictionary<string, string> metadata)
        {
            _metadata = metadata;
        }

        public bool Has(string key)
        {
            return _metadata.ContainsKey(key);
        }

        public string Get(string key)
        {
            return _metadata.ContainsKey(key) ? _metadata[key] : null;
        }
    }
}
