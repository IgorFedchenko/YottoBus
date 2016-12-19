using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Abstract
{
    public interface IEventHandler { }

    public interface IEventHandler<in TEvent> : IEventHandler
    {
        void Handle(TEvent @event, PeerIdentity sender);
    }
}
