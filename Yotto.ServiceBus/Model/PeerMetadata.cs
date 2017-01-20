using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yotto.ServiceBus.Model
{
    /// <summary>
    /// Represents peer metadata
    /// </summary>
    public class PeerMetadata
    {
        [JsonProperty]
        private readonly Dictionary<string, string> _metadata;

        public PeerMetadata() : this(new Dictionary<string, string>()) { }

        public PeerMetadata(Dictionary<string, string> metadata)
        {
            _metadata = metadata;
        }

        /// <summary>
        /// Determines whether peer has specified attribute
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if peer has the specified attribute; otherwise, <c>false</c>.
        /// </returns>
        public bool Has(string key)
        {
            return _metadata.ContainsKey(key);
        }

        /// <summary>
        /// Gets the specified attribute value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Value of attribute or null, if it is not found</returns>
        public string Get(string key)
        {
            return _metadata.ContainsKey(key) ? _metadata[key] : null;
        }
    }
}
