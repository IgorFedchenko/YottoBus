using System;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    public interface IServiceBus
    {
        void Connect(string busEndpointsPattern, PeerIdentity self);
        PeerIdentity[] GetPeers();
        void Subscribe<TEvent>(IEventHandler<TEvent> handler);
        void Unsubscribe<TEvent>(IEventHandler<TEvent> handler);
        void Publish<TEvent>(TEvent @event);
        void Send<TMessage>(TMessage message, PeerIdentity target);
        void Disconnect();
    }
}
