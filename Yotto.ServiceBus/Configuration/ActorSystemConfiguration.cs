using System.Net;
using Akka.Configuration;

namespace Yotto.ServiceBus.Configuration
{
    class ActorSystemConfiguration
    {
        private readonly IPEndPoint _publicEndPoint;

        public ActorSystemConfiguration(IPEndPoint publicEndPoint)
        {
            _publicEndPoint = publicEndPoint;
        }

        public Akka.Configuration.Config GetConfig()
        {
            var config = ConfigurationFactory.ParseString(@"
            akka {  
                actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    }
                remote {
                    helios.tcp {
                        transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                        applied-adapters = []
                        transport-protocol = tcp
                        port = " + _publicEndPoint.Port + @"
                        hostname = localhost
                        public-hostname = " + _publicEndPoint.Address + @"
                    }
                }
            }
            "));
            return config;
        }
    }
}
