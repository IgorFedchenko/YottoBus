using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Abstract
{
    public interface IEventHandler<in TEvent>
    {
        void Handle(TEvent @event);
    }
}
