using System;
using System.Collections.Generic;
using System.Net;
using Akka.Actor;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Helpers;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete
{
    class ServiceBus : IServiceBus
    {
        const string PeerName = "busPeer";

        private readonly PeerConfiguration _configuration;
        private readonly ActorSystem _system;
        private IActorRef _busActor;
        private readonly List<IBusLogger> _loggers = new List<IBusLogger>();

        public ServiceBus(IPEndPoint localEndpoint, TagsList tags, PeerConfiguration configuration)
        {
            _configuration = configuration;
            Self = new PeerIdentity(localEndpoint, tags);

            var systemConfiguration = new ActorSystemConfiguration(localEndpoint);
            _system = ActorSystem.Create("YottoServiceBus", systemConfiguration.GetConfig());
        }

        public PeerIdentity Self { get; }

        public void Connect(EndpointsRange endpointsRange)
        {
            _busActor = _system.ActorOf(Props.Create(() => new ServiceBusActor(endpointsRange.All, Self, TimeSpan.FromMilliseconds(_configuration.PeerResolveTimeout))), PeerName);
        }

        public PeerIdentity[] GetPeers()
        {
           return _busActor.Ask<PeerIdentity[]>(new ServiceBusActor.GetPeers()).Result;
        }

        public void Subscribe<TEvent>(IEventHandler<TEvent> handler)
        {
            _busActor.Tell(new ServiceBusActor.Subscribe(typeof(TEvent), handler));
        }

        public void Unsubscribe<TEvent>(IEventHandler<TEvent> handler)
        {
            _busActor.Tell(new ServiceBusActor.Unsubscribe(typeof(TEvent), handler));
        }

        public void Publish(object @event)
        {
            Publish(@event, TagsList.Empty);
        }

        public void Publish(object @event, string tag)
        {
            Publish(@event, new TagsList(tag));
        }

        public void Publish(object @event, TagsList tags)
        {
            _busActor.Tell(new ServiceBusActor.Publish(@event, tags));
        }

        public void Send(object message, PeerIdentity target)
        {
            _busActor.Tell(new ServiceBusActor.Send(message, target));
        }

        public void AddLogger(IBusLogger logger)
        {
            _loggers.Add(logger);
        }

        public void Disconnect()
        {
            _busActor?.Tell(PoisonPill.Instance);
        }

        public void Dispose()
        {
            Disconnect();
            _system.Terminate();
        }
    }
}