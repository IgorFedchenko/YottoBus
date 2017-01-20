using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete.DeliveryStrategies
{
    /// <summary>
    /// This strategy deliveres messages to subscribers synchronously, one by one per message per subscriber.
    /// </summary>
    /// <seealso cref="Yotto.ServiceBus.Abstract.DeliveryStrategyBase" />
    public class SequentialDeliveryStrategy : DeliveryStrategyBase
    {
        /// <summary>
        /// This method is normally called via IDeliveryStrategy interface <see cref="IDeliveryStrategy" />
        /// </summary>
        /// <param name="message">Message to deliver</param>
        /// <param name="sender">Sender of this message</param>
        /// <param name="subscribers">Subscribers to deliver the message</param>
        public override void DeliverMessage(object message, PeerIdentity sender, IEnumerable<IMessageHandler> subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                HandleMessage(message, sender, subscriber);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialDeliveryStrategy"/> class.
        /// </summary>
        /// <param name="bus">The bus.</param>
        public SequentialDeliveryStrategy(IServiceBus bus) 
            : base(bus)
        {
        }
    }
}
