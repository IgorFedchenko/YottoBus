using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Yotto.ServiceBus.Abstract;

namespace Yotto.ServiceBus.Akka
{
    public static class AkkaExtensions
    {
        public static void SubscribeActorTo<TMessage>(this IPeer peer, IActorRef actor)
        {
            var proxy = new ActorProxy<TMessage>(actor);

            peer.Subscribe(proxy);
        }
    }
}
