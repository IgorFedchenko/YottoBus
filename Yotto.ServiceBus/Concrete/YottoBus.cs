using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Ninject;
using Ninject.Activation;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete
{
    public class YottoBus : IServiceBus
    {
        private static readonly IKernel _container;

        private YottoBus() { }

        static YottoBus()
        {
            _container = new StandardKernel();
            _container.Bind<IServiceBus>().To<YottoBus>().InSingletonScope();
            _container.Bind<IPeer>().To<Peer>();
            _container.Bind<ActorSystem>().ToMethod(BuildSystem).InSingletonScope();
        }

        public List<IBusLogger> Loggers { get; } = new List<IBusLogger>();

        public static IServiceBus Create()
        {
            return _container.Get<IServiceBus>();
        }

        public IPeer CreatePeer(PeerConfiguration configuration)
        {
            var conf = new Ninject.Parameters.ConstructorArgument("configuration", configuration);

            return _container.Get<IPeer>(conf);
        }

        private static ActorSystem BuildSystem(IContext context)
        {
            var config = ConfigurationFactory.ParseString(@"
                akka.suppress-json-serializer-warning = on
            ");

            return ActorSystem.Create("YottoBusSystem", config);
        }
    }
}
