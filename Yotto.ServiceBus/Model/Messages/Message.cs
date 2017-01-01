using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Model.Messages
{
    class Message
    {
        public PeerIdentity Sender { get; set; }

        public object Content { get; set; }
    }
}
