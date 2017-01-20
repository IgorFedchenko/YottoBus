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
    /// This strategy delivers messages asynchronously, 
    /// handling each message on each subscruber in separate thread.
    /// </summary>
    /// <seealso cref="Yotto.ServiceBus.Abstract.DeliveryStrategyBase" />
    public class ParallelDeliveryStrategy : DeliveryStrategyBase
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
                Task.Run(() => HandleMessage(message, sender, subscriber));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelDeliveryStrategy"/> class.
        /// </summary>
        /// <param name="bus">The bus.</param>
        public ParallelDeliveryStrategy(IServiceBus bus)
            : base(bus)
        {
        }
    }
}
