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
    /// <summary>
    /// Implements all high-level functionality
    /// </summary>
    /// <seealso cref="Yotto.ServiceBus.Abstract.IPeer" />
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

        /// <summary>
        /// Gets the identity of this peer.
        /// </summary>
        /// <value>
        /// The identity of this peer.
        /// </value>
        public PeerIdentity Identity { get; }

        /// <summary>
        /// Gets a value indicating whether this peer is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this peer is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Connects this instance to service bus (to local proxy).
        /// </summary>
        public void Connect()
        {
            Connect(19876, 19877);
        }

        /// <summary>
        /// Connects this instance to service bus (to local proxy).
        /// </summary>
        /// <param name="proxyPortForPublishersPort">The proxy port for publishers.</param>
        /// <param name="proxyPortForSubscribers">The proxy port for subscribers.</param>
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

        /// <summary>
        /// Gets the connected peers.
        /// </summary>
        /// <returns>
        /// Connected peers collection
        /// </returns>
        public PeerIdentity[] GetConnectedPeers()
        {
            return _peers.ToArray();
        }

        /// <summary>
        /// Subscribes the specified handler to all message types it is capable to handle.
        /// </summary>
        /// <param name="handler">The handler to be subscrubed.</param>
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

        /// <summary>
        /// Unsubscribes the specified handler from all message types.
        /// </summary>
        /// <param name="handler">The handler to be unsubscrubed.</param>
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

        /// <summary>
        /// Publishes the specified event.
        /// </summary>
        /// <param name="event">The event.</param>
        public void Publish(object @event)
        {
            _publisher.Publish(@event);
        }

        /// <summary>
        /// Sends the specified message to specified peer.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="target">The target.</param>
        public void Send(object message, PeerIdentity target)
        {
            _publisher.Send(message, target);
        }

        /// <summary>
        /// Disconnects this peer from the bus.
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected)
            {
                _connectionTracker.PeerConnected -= HandlePeerConnected;
                _connectionTracker.PeerDisconnected -= HandlePeerDisconnected;

                _publisher.Stop();
                _subscriber.Stop();
                _connectionTracker.Stop();
                IsConnected = false;
            }

            Log(LogLevel.Debug, $"Peer {Identity.Id} disconnected from bus");
        }

        /// <summary>
        /// Disconnects this instance
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }

        /// <summary>
        /// Handles the received message, using setted DeliveryStrategy <see cref="IDeliveryStrategy"/>.
        /// </summary>
        /// <param name="peer">The peer.</param>
        /// <param name="message">The message.</param>
        private void HandleReceivedMessage(PeerIdentity peer, object message)
        {
            try
            {
                bool messageIsBigInt = message is long && (long)message > int.MaxValue;
                if (message is long && !messageIsBigInt)
                {
                    // When message is Int32, after deserializing it has Int64 type (long).
                    // That is why we need to deliver message both to int and long subscribers,
                    // just in a case.
                    HandleReceivedMessage(peer, message);
                    HandleReceivedMessage(peer, Convert.ToInt32(message));
                    return;
                }

                var eventType = message.GetType();
                if (_handlers.ContainsKey(eventType))
                {
                    List<IMessageHandler> handlers = new List<IMessageHandler>();

                    foreach (var handler in _handlers[eventType])
                    {
                        // types which handler is able to handle
                        Type[] handlingTypes = handler.GetType().GetInterfaces()
                            .Where(inf => inf.IsGenericType && inf.GetGenericTypeDefinition() == typeof (IMessageHandler<>))
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
