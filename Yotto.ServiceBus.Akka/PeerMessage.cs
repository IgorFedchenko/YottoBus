using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Akka
{
    public class PeerMessage<TMessage>
    {
        public PeerMessage(TMessage message, PeerIdentity sender)
        {
            Sender = sender;
            Message = message;
        }

        public TMessage Message { get; }
        public PeerIdentity Sender { get; }
    }
}
