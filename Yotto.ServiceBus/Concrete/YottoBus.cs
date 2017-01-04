using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
    }
}
