using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public event Action<PeerIdentity, object> MessageReceived;

        public void SubscribeTo<TMessage>()
        {
            _socket.Subscribe(typeof(TMessage).AssemblyQualifiedName);
        }

        public void UnsubscribeFrom<TMessage>()
        {
            _socket.Unsubscribe(typeof(TMessage).AssemblyQualifiedName);
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
                   
                        var message = JsonConvert.DeserializeObject<Message>(messageString);
                        MessageReceived?.Invoke(message.Sender, message.Content);
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
