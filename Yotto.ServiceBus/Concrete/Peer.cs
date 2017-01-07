using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete
{
    class Peer : IPeer
    {
        private readonly IServiceBus _bus;
        private readonly ISubscriber _subscriber;
        private readonly IPublisher _publisher;
        private readonly IConnectionTracker _connectionTracker;
        private readonly Dictionary<Type, HashSet<IEventHandler>> _handlers = new Dictionary<Type, HashSet<IEventHandler>>();
        private readonly HashSet<PeerIdentity> _peers = new HashSet<PeerIdentity>();

        public Peer(PeerConfiguration configuration, IServiceBus bus, IPublisher publisher, ISubscriber subscriber, IConnectionTracker connectionTracker)
        {
            _bus = bus;
            _subscriber = subscriber;
            _publisher = publisher;
            _connectionTracker = connectionTracker;

            Identity = new PeerIdentity(configuration.Metadata);

            _connectionTracker.PeerConnected += HandlePeerConnected;
            _connectionTracker.PeerDisconnected += HandlePeerDisconnected;

            _subscriber.MessageReceived += msg => HandleReceivedMessage(msg.Sender, msg.Content);
        }

        public PeerIdentity Identity { get; }

        public bool IsConnected { get; private set; }

        public void Connect()
        {
            Connect(19876, 19877);
        }

        public void Connect(int proxyForPublishersPort, int proxyForSubscribersPort)
        {
            if (!IsConnected)
            {
                var publishProxyEndpoint = new IPEndPoint(IPAddress.Loopback, proxyForPublishersPort);
                var subscribeProxyEndpoint = new IPEndPoint(IPAddress.Loopback, proxyForPublishersPort);

                _subscriber.Start(subscribeProxyEndpoint, Identity);
                _publisher.Start(publishProxyEndpoint, Identity);
                _connectionTracker.Start(_publisher, _subscriber, Identity);

                IsConnected = true;

                Log(LogLevel.Debug, $"Peer {Identity.Id} connected to bus via proxy on {proxyForPublishersPort}/{proxyForSubscribersPort} ports");
            }
        }

        public PeerIdentity[] GetConnectedPeers()
        {
            return _peers.ToArray();
        }

        public void Subscribe<TEvent>(IEventHandler<TEvent> handler)
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
                _handlers[eventType] = new HashSet<IEventHandler>();

            _handlers[eventType].Add(handler);

            Log(LogLevel.Trace, $"Peer {Identity.Id} subscribed to {eventType}");
        }

        public void Unsubscribe<TEvent>(IEventHandler<TEvent> handler)
        {
            var eventType = typeof(TEvent);
            if (_handlers.ContainsKey(eventType))
            {
                _handlers[eventType].Remove(handler);
            }

            Log(LogLevel.Trace, $"Peer {Identity.Id} unsubscribed from {eventType}");
        }

        public void Publish(object @event)
        {
            _publisher.Publish(@event);
        }

        public void Send(object message, PeerIdentity target)
        {
            _publisher.Send(message, target);
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                _publisher.Stop();
                _subscriber.Stop();
                _connectionTracker.Stop();
            }

            Log(LogLevel.Debug, $"Peer {Identity.Id} disconnected from bus");
        }

        public void Dispose()
        {
            _connectionTracker.PeerConnected -= HandlePeerConnected;
            _connectionTracker.PeerDisconnected -= HandlePeerDisconnected;

            Disconnect();
        }

        private void HandleReceivedMessage(PeerIdentity peer, object message)
        {
            try
            {
                var eventType = message.GetType();
                if (_handlers.ContainsKey(eventType))
                {
                    foreach (var handler in _handlers[eventType])
                    {
                        Type[] handlingTypes = handler.GetType().GetInterfaces()
                            .Where(
                                inf => inf.IsGenericType && inf.GetGenericTypeDefinition() == typeof (IEventHandler<>))
                            .Select(inf => inf.GetGenericArguments().First())
                            .ToArray();

                        if (handlingTypes.Any(t => t == eventType || eventType.IsSubclassOf(t)))
                        {
                            // If handling this type of messages
                            handler.GetType()
                                .GetMethod(nameof(IEventHandler<object>.Handle), new Type[] {eventType})
                                .Invoke(handler, new object[] {message, peer});
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, $"Failed to handle message {JsonConvert.SerializeObject(message)}: {ex}");
            }
        }

        private void HandlePeerConnected(PeerIdentity peer)
        {
            _peers.Add(peer);

            Log(LogLevel.Trace, $"Connected peer {peer.Id}");
        }

        private void HandlePeerDisconnected(PeerIdentity peer)
        {
            _peers.Remove(peer);

            Log(LogLevel.Trace, $"Disconnected peer {peer.Id}");
        }

        private void Log(LogLevel level, string message)
        {
            foreach (var busLogger in _bus.Loggers.ToArray())
            {
                busLogger.Log(level, message);
            }
        }
    }
}
