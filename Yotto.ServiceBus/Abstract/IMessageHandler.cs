using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    public interface IMessageHandler { }

    public interface IMessageHandler<in TMessage> : IMessageHandler
    {
        void Handle(TMessage @event, PeerIdentity sender);
    }
}
