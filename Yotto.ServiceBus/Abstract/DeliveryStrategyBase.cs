using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    /// <summary>
    /// This is a basis for all DeliveryStrategies you may want to implement. 
    /// Usually you will inherit from this class and implement some strategy, 
    /// calling HandleMessage <see cref="HandleMessage"/> to invoke handler method synchronously.
    /// </summary>
    public abstract class DeliveryStrategyBase
    {
        private readonly List<IBusLogger> _loggers = new List<IBusLogger>();

        /// <summary>
        /// Delivers the message to specified subscribers.
        /// </summary>
        /// <param name="message">The message to delivered.</param>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="subscribers">The subscribers to deliver.</param>
        public abstract void DeliverMessage(object message, PeerIdentity sender, IEnumerable<IMessageHandler> subscribers);

        /// <summary>
        /// Adds loggers to be used for logging delivery events
        /// </summary>
        /// <param name="loggers">The loggers.</param>
        internal void AddLoggers(IEnumerable<IBusLogger> loggers)
        {
            _loggers.AddRange(loggers);
        }

        /// <summary>
        /// Delivers the message to particular subscriber/handler, calling it's handling method
        /// </summary>
        /// <param name="message">Message to handle</param>
        /// <param name="sender">Sender of the message</param>
        /// <param name="handler">Handler, who's handler method will be invoked</param>
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
