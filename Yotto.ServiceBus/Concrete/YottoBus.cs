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
    public class YottoBus : IServiceBus
    {
        private YottoBus() { }

        public static IServiceBus Create()
        {
            return new YottoBus();
        }

        public List<IBusLogger> Loggers { get; } = new List<IBusLogger>();

        public IPeer CreatePeer(PeerConfiguration configuration)
        {
            return new Peer(configuration, this);
        }
    }
}
