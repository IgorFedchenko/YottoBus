using System;
using System.Collections.Generic;
using System.Net;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    public interface IServiceBus : IDisposable
    {
        void Connect(string busEndpointsPattern);
        void Connect(List<IPEndPoint> busEndpoints);

        PeerIdentity Self { get; }
        PeerIdentity[] GetPeers();

        void Subscribe<TEvent>(IEventHandler<TEvent> handler);
        void Unsubscribe<TEvent>(IEventHandler<TEvent> handler);

        void Publish(object @event);
        void Publish(object @event, string tag);
        void Publish(object @event, TagsList tags);

        void Send(object message, PeerIdentity target);

        void AddLogger(IBusLogger logger);

        void Disconnect();
    }
}
