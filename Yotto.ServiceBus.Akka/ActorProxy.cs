using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Akka
{
    class ActorProxy<TMessage> : IMessageHandler<TMessage>
    {
        private readonly IActorRef _actor;

        public ActorProxy(IActorRef actor)
        {
            _actor = actor;
        }

        public void Handle(TMessage @event, PeerIdentity sender)
        {
            _actor.Tell(new PeerMessage<TMessage>(@event, sender), ActorRefs.NoSender);
        }
    }
}
