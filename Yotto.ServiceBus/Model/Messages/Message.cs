using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yotto.ServiceBus.Model.Messages
{
    /// <summary>
    /// Wrapper for all messages, sent on the bus
    /// </summary>
    class Message
    {
        /// <summary>
        /// Sender of the message.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        public PeerIdentity Sender { get; set; }

        /// <summary>
        /// Body of the message. Contains all sent information.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public object Content { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        /// <summary>
        /// Parses the serialized message.
        /// </summary>
        /// <param name="messageString">The serialized message.</param>
        /// <returns></returns>
        public static Message Parse(string messageString)
        {
            return JsonConvert.DeserializeObject<Message>(messageString, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }
}
