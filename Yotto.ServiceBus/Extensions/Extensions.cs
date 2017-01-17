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
        public static IPeer CreatePeer(this IServiceBus bus)
        {
            return bus.CreatePeer(new PeerConfiguration());
        }

        public static IPeer CreatePeer(this IServiceBus bus, string peerName)
        {
            return bus.CreatePeer(new PeerConfiguration()
            {
                Metadata = new PeerMetadata(new Dictionary<string, string>()
                {
                    ["name"] = peerName,
                })
            });
        }

        public static IPeer CreatePeer(this IServiceBus bus, string peerName, Dictionary<string, string> attributes)
        {
            attributes.Remove("name");

            return bus.CreatePeer(new PeerConfiguration()
            {
                Metadata = new PeerMetadata(new Dictionary<string, string>()
                {
                    ["name"] = peerName,
                }.Concat(attributes).ToDictionary(p => p.Key, p => p.Value))
            });
        }

        public static string GetName(this IPeer peer)
        {
            return peer.Identity.Metadata.Get("name") ?? string.Empty;
        }
    }
}
