using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Configuration
{
    public class PeerConfiguration
    {
        public int PeerResolveTimeout { get; set; } = 5000;
    }
}
