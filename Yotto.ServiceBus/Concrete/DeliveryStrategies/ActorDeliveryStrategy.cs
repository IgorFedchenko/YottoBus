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
    public class ActorDeliveryStrategy : DeliveryStrategyBase
    {
        private readonly ConcurrentDictionary<IMessageHandler, Task> _handlersTasks = new ConcurrentDictionary<IMessageHandler, Task>(); 

        public ActorDeliveryStrategy(IEnumerable<IBusLogger> loggers) 
            : base(loggers)
        {
        }

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
