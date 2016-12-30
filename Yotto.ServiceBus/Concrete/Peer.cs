using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Concrete.Actors;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete
{
    class Peer : IPeer
    {
        private readonly ActorSystem _system;
        private IActorRef _actor = ActorRefs.Nobody;
        private readonly IServiceBus _bus;
        private object _connectLock = new object();

        public Peer(PeerConfiguration configuration, ActorSystem system, IServiceBus bus)
        {
            _system = system;
            _bus = bus;

            Identity = new PeerIdentity(configuration.Metadata);
        }

        public PeerIdentity Identity { get; }

        public bool IsConnected { get; private set; }

        public void Connect()
        {
            lock (_connectLock)
            {
                if (!IsConnected)
                {
                    _actor = _system.ActorOf(Props.Create<PeerActor>());
                    IsConnected = true;
                }
            }
        }

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
            lock (_connectLock)
            {
                if (IsConnected)
                {
                    _actor.Tell(PoisonPill.Instance);
                    _actor = ActorRefs.Nobody;
                }
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
