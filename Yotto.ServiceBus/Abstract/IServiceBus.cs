using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Configuration;

namespace Yotto.ServiceBus.Abstract
{
    public interface IServiceBus
    {
        List<IBusLogger> Loggers { get; }

        IDeliveryStrategy DeliveryStrategy { get; set; }

        IPeer CreatePeer(PeerConfiguration configuration);

    }
}
