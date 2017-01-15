using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    public interface IDeliveryStrategy
    {
        void DeliverMessage(object message, PeerIdentity sender, IEnumerable<IMessageHandler> subscribers);
    }
}
