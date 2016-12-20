using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete
{
    public class BusConnectionFactory
    {
        public static IServiceBus CreateClient(IPEndPoint localEndpoint, TagsList tags, PeerConfiguration configuration)
        {
            return new ServiceBus(localEndpoint, tags, configuration);
        }
    }
}
