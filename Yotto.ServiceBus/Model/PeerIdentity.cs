using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Model
{
    public abstract class PeerIdentity
    {
        public abstract Guid Id { get; }
    }
}
