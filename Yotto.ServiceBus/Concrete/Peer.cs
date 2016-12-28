using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete
{
    class Peer : IPeer
    {
        private readonly IServiceBus _bus;

        public Peer(PeerConfiguration configuration, IServiceBus bus)
        {
            _bus = bus;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public bool IsConnected { get; }

        public PeerIdentity Identity { get; }

        public PeerIdentity[] GetConnectedPeers()
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TEvent>(IEventHandler<TEvent> handler)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEvent>(IEventHandler<TEvent> handler)
        {
            throw new NotImplementedException();
        }

        public void Publish(object @event)
        {
            throw new NotImplementedException();
        }

        public void Send(object message, PeerIdentity target)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }
    }
}
