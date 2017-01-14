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
    class Subscriber : ISubscriber
    {
        private SubscriberSocket _socket;
        private CancellationTokenSource _cancellation;

        public event Action<Message> MessageReceived;

        public void SubscribeTo(Type messageType)
        {
            _socket.Subscribe(messageType.AssemblyQualifiedName);
        }

        public void UnsubscribeFrom(Type messageType)
        {
            _socket.Unsubscribe(messageType.AssemblyQualifiedName);
        }

        public void Start(int bindPort, PeerIdentity peer)
        {
            _socket = new SubscriberSocket();
            _socket.Bind("tcp://localhost:" + bindPort);

            _socket.Subscribe(peer.Id.ToString());
        }

        public void Start(IPEndPoint publisher, PeerIdentity peer)
        {
            Start(new List<IPEndPoint>() { publisher }, peer);
        }

        public void Start(List<IPEndPoint> publishers, PeerIdentity peer)
        {
            _socket = new SubscriberSocket();
            foreach (var publisherEndpoint in publishers)
            {
                var endpoint = $"tcp://{publisherEndpoint.Address}:{publisherEndpoint.Port}";
                _socket.Connect(endpoint);
            }

            StartReceivingMessages();
        }

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
                            ((Action<Message>) @delegate)?.BeginInvoke(message, null, null);
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
