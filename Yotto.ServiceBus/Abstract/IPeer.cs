using System;
using System.Collections.Generic;
using System.Net;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    /// <summary>
    /// Describes single peer in the service bus.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IPeer : IDisposable
    {
        /// <summary>
        /// Connects this instance to service bus (to local proxy).
        /// </summary>
        void Connect();

        /// <summary>
        /// Gets a value indicating whether this peer is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this peer is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the identity of this peer.
        /// </summary>
        /// <value>
        /// The identity of this peer.
        /// </value>
        PeerIdentity Identity { get; }

        /// <summary>
        /// Gets the connected peers.
        /// </summary>
        /// <returns>Connected peers collection</returns>
        PeerIdentity[] GetConnectedPeers();

        /// <summary>
        /// Subscribes the specified handler to all message types it is capable to handle.
        /// </summary>
        /// <param name="handler">The handler to be subscrubed.</param>
        void Subscribe(IMessageHandler handler);

        /// <summary>
        /// Unsubscribes the specified handler from all message types.
        /// </summary>
        /// <param name="handler">The handler to be unsubscrubed.</param>
        void Unsubscribe(IMessageHandler handler);

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        void Publish(object message);

        /// <summary>
        /// Sends the specified message to specified peer.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="target">The target.</param>
        void Send(object message, PeerIdentity target);

        /// <summary>
        /// Disconnects this peer from the bus.
        /// </summary>
        void Disconnect();
    }
}
