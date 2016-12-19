using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Akka.Configuration;

namespace Yotto.ServiceBus.Model
{
    class ActorSystemConfiguration
    {
        public ActorSystemConfiguration(IPEndPoint publicEndPoint)
        {
            
        }

        public Akka.Configuration.Config GetConfig()
        {
            return ConfigurationFactory.ParseString(@"
                akka.suppress-json-serializer-warning = on
            ");
        }
    }
}
