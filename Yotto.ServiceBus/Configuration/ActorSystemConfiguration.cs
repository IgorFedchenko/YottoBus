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
            var config = @"
            akka {  
                actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                        debug {
                          receive = on
                          autoreceive = on
                          lifecycle = on
                          event-stream = on
                          unhandled = on
                        }
                    }
                remote {
                    log-received-messages = on
                    helios.tcp {
                        transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                        applied-adapters = []
                        transport-protocol = tcp
                        port = " + _publicEndPoint.Port + @"
                        hostname = ""0.0.0.0""
                        public-hostname = """ + _publicEndPoint.Address + @"""
                    }
                }
            }";
            return ConfigurationFactory.ParseString(config);
        }
    }
}
