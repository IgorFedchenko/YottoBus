using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;
using Yotto.ServiceBus.Model.Messages;

namespace Yotto.ServiceBus.Concrete
{
    class Peer : IPeer
    {
        private readonly IServiceBus _bus;
        private readonly ISubscriber _subscriber;
        private readonly IPublisher _publisher;
        private readonly IConnectionTracker _connectionTracker;
        private readonly Dictionary<Type, HashSet<IMessageHandler>> _handlers = new Dictionary<Type, HashSet<IMessageHandler>>();
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

        public void Connect(int proxyPortForPublishersPort, int proxyPortForSubscribers)
        {
            if (!IsConnected)
            {
                var publishProxyEndpoint = new IPEndPoint(IPAddress.Loopback, proxyPortForPublishersPort);
                var subscribeProxyEndpoint = new IPEndPoint(IPAddress.Loopback, proxyPortForSubscribers);

                _subscriber.Start(subscribeProxyEndpoint, Identity);
                _publisher.Start(publishProxyEndpoint, Identity);
                _connectionTracker.Start(_publisher, _subscriber, Identity);

                IsConnected = true;

                Log(LogLevel.Debug, $"Peer {Identity.Id} initiated connection to bus via proxy on {proxyPortForPublishersPort}/{proxyPortForSubscribers} ports");
            }
        }

        public PeerIdentity[] GetConnectedPeers()
        {
            return _peers.ToArray();
        }

        public void Subscribe(IMessageHandler handler)
        {
            Type[] handlingTypes = handler.GetType().GetInterfaces()
                            .Where(
                                inf => inf.IsGenericType && inf.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                            .Select(inf => inf.GetGenericArguments().First())
                            .ToArray();

            foreach (var messageType in handlingTypes)
            {
                if (!_handlers.ContainsKey(messageType))
                    _handlers[messageType] = new HashSet<IMessageHandler>();

                _handlers[messageType].Add(handler);

                _subscriber.SubscribeTo(messageType);
            }
        }

        public void Unsubscribe(IMessageHandler handler)
        {
            Type[] handlingTypes = handler.GetType().GetInterfaces()
                            .Where(
                                inf => inf.IsGenericType && inf.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                            .Select(inf => inf.GetGenericArguments().First())
                            .ToArray();

            foreach (var messageType in handlingTypes)
            {
                if (_handlers.ContainsKey(messageType))
                {
                    _handlers[messageType].Remove(handler);
                    if (!_handlers[messageType].Any())
                    {
                        _handlers.Remove(messageType);
                    }
                }

                if (!_handlers.ContainsKey(messageType))
                {
                    _subscriber.UnsubscribeFrom(messageType);
                }

                Log(LogLevel.Trace, $"Peer {Identity.Id} unsubscribed from {messageType}");
            }
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
                IsConnected = false;
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
                bool messageIsBigInt = message is long && (long)message > int.MaxValue;
                if (message is long && !messageIsBigInt)
                {
                    HandleReceivedMessage(peer, Convert.ToInt32(message));
                    return;
                }

                var eventType = message.GetType();
                if (_handlers.ContainsKey(eventType))
                {
                    List<IMessageHandler> handlers = new List<IMessageHandler>();

                    foreach (var handler in _handlers[eventType])
                    {
                        Type[] handlingTypes = handler.GetType().GetInterfaces()
                            .Where(
                                inf => inf.IsGenericType && inf.GetGenericTypeDefinition() == typeof (IMessageHandler<>))
                            .Select(inf => inf.GetGenericArguments().First())
                            .ToArray();

                        if (handlingTypes.Any(t => t == eventType || eventType.IsSubclassOf(t)))
                        {
                            // If handling this type of messages
                            handlers.Add(handler);
                        }
                    }

                     _bus.DeliveryStrategy.DeliverMessage(message, peer, handlers);
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

            HandleReceivedMessage(Identity, new PeerConnected(peer));
        }

        private void HandlePeerDisconnected(PeerIdentity peer)
        {
            _peers.Remove(peer);

            Log(LogLevel.Trace, $"Disconnected peer {peer.Id}");

            HandleReceivedMessage(Identity, new PeerDisconnected(peer));
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
