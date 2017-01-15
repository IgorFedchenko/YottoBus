using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    public abstract class DeliveryStrategyBase : IDeliveryStrategy
    {
        private readonly IEnumerable<IBusLogger> _loggers;

        protected DeliveryStrategyBase(IEnumerable<IBusLogger> loggers)
        {
            _loggers = loggers;
        }

        public abstract void DeliverMessage(object message, PeerIdentity sender, IEnumerable<IMessageHandler> subscribers);

        protected void HandleMessage(object message, PeerIdentity sender, IMessageHandler handler)
        {
            try
            {
                var messageType = message.GetType();
                var method = handler.GetType()
                    .GetMethod(nameof(IMessageHandler<object>.Handle), new Type[] {messageType, typeof (PeerIdentity)});
                method.Invoke(handler, new[] {message, sender});
            }
            catch (Exception ex)
            {
                LogError($"Error while handling message {message} from peer {sender}", ex);
            }
        }

        private void LogError(string message, Exception ex)
        {
            foreach (var logger in _loggers.ToArray())
            {
                logger.Error(message, ex);
            }
        }
    }
}
