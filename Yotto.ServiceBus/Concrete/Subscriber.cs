using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    /// Implements subscriber logic
    /// </summary>
    /// <seealso cref="Yotto.ServiceBus.Abstract.ISubscriber" />
    class Subscriber : ISubscriber
    {
        private readonly MessageTopicBuilder _topicBuilder;
        private SubscriberSocket _socket;
        private CancellationTokenSource _cancellation;
        private PeerIdentity _peer;

        /// <summary>
        /// Occurs when new message received.
        /// </summary>
        public event Action<Message> MessageReceived;

        public Subscriber(MessageTopicBuilder topicBuilder)
        {
            _topicBuilder = topicBuilder;
        }


        /// <summary>
        /// Add subscrubtion to given message type
        /// </summary>
        /// <param name="messageType">Type of the message to subscrube.</param>
        public void SubscribeTo(Type messageType)
        {
            _socket.Subscribe(_topicBuilder.GetMessageTag(_peer.Context, messageType));
        }

        /// <summary>
        /// Unsubscribes from specified message type.
        /// </summary>
        /// <param name="messageType">Type of the message to unsubscrube.</param>
        public void UnsubscribeFrom(Type messageType)
        {
            _socket.Unsubscribe(_topicBuilder.GetMessageTag(_peer.Context, messageType));
        }

        /// <summary>
        /// Starts this instance, connecting it to specified publisher.
        /// </summary>
        /// <param name="publisher">The publisher to connect.</param>
        /// <param name="peer">The self identity.</param>
        public void Start(IPEndPoint publisher, PeerIdentity peer)
        {
            Start(new List<IPEndPoint>() { publisher }, peer);
        }

        /// <summary>
        /// Starts this instance, connecting it to specified publishers.
        /// </summary>
        /// <param name="publishers">The publishers to connect.</param>
        /// <param name="peer">The self peer identity.</param>
        public void Start(List<IPEndPoint> publishers, PeerIdentity peer)
        {
            _peer = peer;

            _socket = new SubscriberSocket();
            foreach (var publisherEndpoint in publishers)
            {
                var endpoint = $"tcp://{publisherEndpoint.Address}:{publisherEndpoint.Port}";
                _socket.Connect(endpoint);
            }

            _socket.Subscribe(_topicBuilder.GetMessageTag(peer.Context, peer.Id));

            StartReceivingMessages();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _cancellation.Cancel();
            _socket.Close();
        }

        private void StartReceivingMessages()
        {
            _cancellation = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (!_cancellation.IsCancellationRequested)
                {
                    try
                    {
                        var topic = _socket.ReceiveFrameString();
                        var messageString = _socket.ReceiveFrameString();

                        var message = Message.Parse(messageString);

                        foreach (var @delegate in MessageReceived?.GetInvocationList().ToArray() ?? new Delegate[0])
                        {
                            ((Action<Message>) @delegate)?.BeginInvoke(message, null, null); // Do not wait while message will be handled
                        }
                    }
                    catch (Exception ex) when (ex is ObjectDisposedException || ex is SocketException)
                    {
                        // Ignore this one: _socket may be closed while waiting the message and this is fine
                    }
                    catch (Exception ex)
                    {
                        // TODO: Log Exception
                        Console.WriteLine("Error while receiving the message: " + ex);
                    }
                }
            }, _cancellation.Token);
        }
    }
}
