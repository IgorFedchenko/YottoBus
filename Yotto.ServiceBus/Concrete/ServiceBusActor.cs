using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Newtonsoft.Json;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete
{
    class ServiceBusActor : ReceiveActor
    {
        #region Message types

        public class GetPeers { }

        public class Subscribe
        {
            public Subscribe(Type eventType, IEventHandler handler)
            {
                EventType = eventType;
                Handler = handler;
            }

            public Type EventType { get; }
            public IEventHandler Handler { get; } 
        }

        public class Unsubscribe
        {
            public Unsubscribe(Type eventType, IEventHandler handler)
            {
                EventType = eventType;
                Handler = handler;
            }

            public Type EventType { get; }
            public IEventHandler Handler { get; }
        }

        public class Publish
        {
            public Publish(object @event, TagsList tags)
            {
                Event = @event;
                Tags = tags;
            }

            public object Event { get; }
            public TagsList Tags { get; }
        }

        public class Send
        {
            public Send(object message, PeerIdentity target)
            {
                Message = message;
                Target = target;
            }

            public object Message { get; }
            public PeerIdentity Target { get; }
        }

        public class GetIdentity { }

        public class CheckEndpoint
        {
            public CheckEndpoint(IPEndPoint endpoint)
            {
                Endpoint = endpoint;
            }

            public IPEndPoint Endpoint { get; }
        }

        #endregion

        private readonly PeerIdentity _selfIdentity;
        private readonly TimeSpan _resolveTimeout;
        private readonly Dictionary<Type, HashSet<IEventHandler>> _handlers = new Dictionary<Type, HashSet<IEventHandler>>();
        private readonly Dictionary<PeerIdentity, IActorRef> _peers = new Dictionary<PeerIdentity, IActorRef>();

        public ServiceBusActor(List<IPEndPoint> busEndpoints, PeerIdentity selfIdentity, TimeSpan resolveTimeout)
        {
            _selfIdentity = selfIdentity;
            _resolveTimeout = resolveTimeout;

            Receive<GetIdentity>(msg => HandleIdentityRequest(msg));
            Receive<PeerIdentity>(msg => HandleConnected(msg, Sender));
            Receive<CheckEndpoint>(msg => TryResolveEndpoint(msg.Endpoint));
            Receive<Terminated>(msg => HandleDisconnected(msg.ActorRef));

            Receive<Subscribe>(msg => AddHandler(msg));
            Receive<Unsubscribe>(msg => RemoveHandler(msg));
            Receive<Publish>(msg => PublishMessage(msg));
            Receive<Send>(msg => SendMessageToPeer(msg.Message, msg.Target));

            Receive<GetPeers>(msg => Sender.Tell(_peers.Keys.ToArray()));

            ReceiveAny(_ => HandleConnected(null, Self));

            foreach (var busEndpoint in busEndpoints)
            {
                Self.Tell(new CheckEndpoint(busEndpoint));
            }
        }

        private void TryResolveEndpoint(IPEndPoint endPoint)
        {
            string address = $"akka.tcp://{Context.System.Name}@{endPoint.Address}:{endPoint.Port}/user/{Self.Path.Name}";
            var self = Self;
            Context.System.ActorSelection(address)
                .ResolveOne(TimeSpan.FromMilliseconds(1000))
                .ContinueWith(
                    task =>
                    {
                        if (task.IsFaulted)
                            self.Tell(new CheckEndpoint(endPoint), self);
                        else
                            task.Result.Tell(new GetIdentity(), self);
                    });
        }

        private void HandleConnected(PeerIdentity identity, IActorRef peer)
        {
            Context.Watch(peer);

            _peers[identity] = peer;

            InvokeSubscriptions(new PeerConected(identity), _selfIdentity);
        }

        private void HandleDisconnected(IActorRef peer)
        {
            var identity = GetIdentityOf(peer);
            InvokeSubscriptions(new PeerDisconnected(identity), _selfIdentity);

            _peers.Remove(identity);

            TryResolveEndpoint(GetActorEndpoint(peer));
        }

        private void HandleIdentityRequest(GetIdentity msg)
        {
            Sender.Tell(_selfIdentity);
        }

        private void AddHandler(Subscribe msg)
        {
            if (!_handlers.ContainsKey(msg.EventType))
                _handlers[msg.EventType] = new HashSet<IEventHandler>();

            _handlers[msg.EventType].Add(msg.Handler);
        }

        private void RemoveHandler(Unsubscribe msg)
        {
            if (_handlers.ContainsKey(msg.EventType))
            {
                _handlers[msg.EventType].Remove(msg.Handler);
            }
        }

        private void PublishMessage(Publish msg)
        {
            foreach (var peer in _peers.Keys.Where(p => p.Tags.AllTags.Intersect(msg.Tags.AllTags).Any()))
            {
                _peers[peer].Tell(msg.Event);
            }
        }

        private void SendMessageToPeer(object message, PeerIdentity peer)
        {
            _peers[peer].Tell(message);
        }

        private void InvokeSubscriptions(object @event, PeerIdentity sender)
        {
             var eventType = @event.GetType();
            if (_handlers.ContainsKey(eventType))
            {
                foreach (var handler in _handlers[eventType])
                {
                    Type[] handlingTypes = handler.GetType().GetInterfaces()
                     .Where(inf => inf.IsGenericType && inf.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                     .Select(inf => inf.GetGenericArguments().First())
                     .ToArray();

                    if (handlingTypes.Any(t => t == eventType || eventType.IsSubclassOf(t)))
                    {
                        // If handling this type of messages
                        handler.GetType().GetMethod(nameof(IEventHandler<object>.Handle), new Type[] { eventType }).Invoke(handler, new object[] { @event, sender });
                    }
                }
            }
        }

        private PeerIdentity GetIdentityOf(IActorRef peer)
        {
            return _peers.First(pair => Equals(pair.Value, peer)).Key;
        }

        private IPEndPoint GetActorEndpoint(IActorRef actor)
        {
            return new IPEndPoint(IPAddress.Parse(actor.Path.Address.Host), (int)actor.Path.Address.Port);
        }
    }
}
