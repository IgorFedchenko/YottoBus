using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Model;
using Yotto.ServiceBus.Model.Messages;

namespace Yotto.ServiceBus.Concrete
{
    /// <summary>
    /// Implements publisher logic
    /// </summary>
    /// <seealso cref="Yotto.ServiceBus.Abstract.IPublisher" />
    class Publisher : IPublisher
    {
        private readonly MessageTopicBuilder _topicBuilder;
        private PublisherSocket _socket;
        private PeerIdentity _selfIdentity;

        public Publisher(MessageTopicBuilder topicBuilder)
        {
            _topicBuilder = topicBuilder;
        }

        /// <summary>
        /// Starts the publisher, connecting it to specified subscruber.
        /// </summary>
        /// <param name="subscriber">The subscriber to connect.</param>
        /// <param name="selfIdentity">The self identity.</param>
        public void Start(IPEndPoint subscriber, PeerIdentity selfIdentity)
        {
            Start(new List<IPEndPoint>() { subscriber }, selfIdentity);
        }

        /// <summary>
        /// Starts the publisher, connecting it to specified subscrubers.
        /// </summary>
        /// <param name="subscribers">The subscribers to connect.</param>
        /// <param name="selfIdentity">The self identity.</param>
        public void Start(List<IPEndPoint> subscribers, PeerIdentity selfIdentity)
        {
            _selfIdentity = selfIdentity;

            _socket = new PublisherSocket();

            foreach (var subscriber in subscribers)
            {
                _socket.Connect($"tcp://{subscriber.Address}:{subscriber.Port}");
            }
        }

        /// <summary>
        /// Stops this publisher.
        /// </summary>
        public void Stop()
        {
            _socket.Close();
        }

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        public void Publish(object message)
        {
            string topic = _topicBuilder.GetMessageTag(_selfIdentity.Context, message.GetType()); // Use message type as a topic
            string serializedMessage = WrapMessage(message).ToString();

            _socket.SendMoreFrame(Encode(topic)).SendFrame(Encode(serializedMessage));
        }

        /// <summary>
        /// Sends the specified message to specified peer.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="peer">The peer to send to</param>
        public void Send(object message, PeerIdentity peer)
        {
            string topic = _topicBuilder.GetMessageTag(_selfIdentity.Context, peer.Id); // Use peer Id as a topic
            string serializedMessage = WrapMessage(message).ToString();

            _socket.SendMoreFrame(Encode(topic)).SendFrame(Encode(serializedMessage));
        }

        private byte[] Encode(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private Message WrapMessage(object message)
        {
            return new Message()
            {
                Content = message,
                Sender = _selfIdentity
            };
        }
    }
}
