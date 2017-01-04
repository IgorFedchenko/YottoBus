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
    class Publisher : IPublisher
    {
        private PublisherSocket _socket;
        private PeerIdentity _selfIdentity;

        public void Start(int bindPort, PeerIdentity selfIdentity)
        {
            _selfIdentity = selfIdentity;

            _socket = new PublisherSocket();
            _socket.Bind("tcp://localhost:" + bindPort);
        }

        public void Start(IPEndPoint subscriber, PeerIdentity selfIdentity)
        {
            Start(new List<IPEndPoint>() { subscriber }, selfIdentity);
        }

        public void Start(List<IPEndPoint> subscribers, PeerIdentity selfIdentity)
        {
            _selfIdentity = selfIdentity;

            _socket = new PublisherSocket();

            foreach (var subscriber in subscribers)
            {
                _socket.Connect($"tcp://{subscriber.Address}:{subscriber.Port}");
            }

        }

        public void Stop()
        {
            _socket.Close();
        }

        public void Publish(object message)
        {
            string topic = message.GetType().AssemblyQualifiedName;
            string serializedMessage = JsonConvert.SerializeObject(WrapMessage(message));

            _socket.SendMoreFrame(Encode(topic)).SendFrame(Encode(serializedMessage));
        }

        public void Send(object message, PeerIdentity peer)
        {
            string topic = peer.Id.ToString();
            string serializedMessage = JsonConvert.SerializeObject(WrapMessage(message));

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
