using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Configuration;

namespace Yotto.ServiceBus.Extensions
{
    public static class Extensions
    {
        public static IPeer CreatePeer(this IServiceBus bus)
        {
            return bus.CreatePeer(new PeerConfiguration());
        }
    }
}
