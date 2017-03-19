using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete.DeliveryStrategies
{
    /// <summary>
    /// This delivery strategy works like in actor model: 
    /// messages are delivered for each subscruber asynchronously, 
    /// but inside each subscruber they are handled synchronously.
    /// 
    /// So therefore subscrubers are handling messages in parallel, 
    /// but each of them has it's message queue to handle messages sequencially.
    /// </summary>
    /// <remarks>Unlike in actor model, each message in subscriber may be handled in different thread. 
    /// All, what is guaranteed, is that next message will not start handling 
    /// before previose message handling is completed.</remarks>
    /// <seealso cref="Yotto.ServiceBus.Abstract.DeliveryStrategyBase" />
    public class ActorDeliveryStrategy : DeliveryStrategyBase
    {
        /// <summary>
        /// Tasks for each handler
        /// </summary>
        private readonly ConcurrentDictionary<IMessageHandler, Task> _handlersTasks = new ConcurrentDictionary<IMessageHandler, Task>();

        /// <summary>
        /// This method is normally called via IDeliveryStrategy interface <see cref="IDeliveryStrategy" />
        /// </summary>
        /// <param name="message">Message to deliver</param>
        /// <param name="sender">Sender of this message</param>
        /// <param name="subscribers">Subscribers to deliver the message</param>
        public override void DeliverMessage(object message, PeerIdentity sender, IEnumerable<IMessageHandler> subscribers)
        {
            ClearFinishedTasks();

            foreach (var subscriber in subscribers)
            {
                _handlersTasks.AddOrUpdate(subscriber, 
                    handler =>
                    {
                        return Task.Run(() => HandleMessage(message, sender, subscriber));
                    },
                    (handler, task) =>
                    {
                        return task.ContinueWith(t => HandleMessage(message, sender, subscriber));
                    });
            }
        }

        /// <summary>
        /// Clears the finished tasks to exclude memory leaks.
        /// </summary>
        private void ClearFinishedTasks()
        {
            foreach (var handlerTask in _handlersTasks.Where(handlerTask => handlerTask.Value.IsCompleted).ToArray())
            {
                Task tmp;
                _handlersTasks.TryRemove(handlerTask.Key, out tmp);
            }
        }
    }
}
