using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Creates the peer with default configuration.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <returns>Created peer</returns>
        public static IPeer CreatePeer(this IServiceBus bus)
        {
            return bus.CreatePeer(new PeerConfiguration(string.Empty));
        }

        /// <summary>
        /// Creates the peer with default configuration.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="context">Context to put peer into</param>
        /// <returns>Created peer</returns>
        public static IPeer CreatePeer(this IServiceBus bus, string context)
        {
            return bus.CreatePeer(new PeerConfiguration(context));
        }

        /// <summary>
        /// Creates the peer with it's name in metadata.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="context">Context to put peer into</param>
        /// <param name="peerName">Name of the peer.</param>
        /// <returns>Created peer</returns>
        public static IPeer CreatePeer(this IServiceBus bus, string context, string peerName)
        {
            return CreatePeer(bus, context, peerName, new Dictionary<string, string>());
        }

        /// <summary>
        /// Creates the peer with it's name and other attributes in metadata.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="context">Context to put peer into</param>
        /// <param name="peerName">Name of the peer.</param>
        /// <param name="attributes">The other attributes.</param>
        /// <returns>Created peer</returns>
        public static IPeer CreatePeer(this IServiceBus bus, string context, string peerName, Dictionary<string, string> attributes)
        {
            attributes.Remove("__name__");

            return bus.CreatePeer(new PeerConfiguration(context, new Dictionary<string, string>()
            {
                ["__name__"] = peerName,
            }.Concat(attributes).ToDictionary(p => p.Key, p => p.Value)));
        }

        /// <summary>
        /// Gets the name of the peer from it's metadata. Empty string by default.
        /// </summary>
        /// <param name="peer">The peer.</param>
        /// <returns>Peer's name, if any (or empty string)</returns>
        public static string GetName(this IPeer peer)
        {
            return peer.Identity.Metadata.Get("__name__") ?? string.Empty;
        }
    }
}
