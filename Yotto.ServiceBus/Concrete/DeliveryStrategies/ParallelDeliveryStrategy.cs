using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete.DeliveryStrategies
{
    public class ParallelDeliveryStrategy : DeliveryStrategyBase
    {
        public ParallelDeliveryStrategy(IEnumerable<IBusLogger> loggers) 
            : base(loggers)
        {
        }

        public override void DeliverMessage(object message, PeerIdentity sender, IEnumerable<IMessageHandler> subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                Task.Run(() => HandleMessage(message, sender, subscriber));
            }
        }
    }
}
