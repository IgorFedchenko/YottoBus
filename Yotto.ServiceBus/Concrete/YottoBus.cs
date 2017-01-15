using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Activation;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Concrete.Loggers;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete
{
    public class YottoBus : IServiceBus
    {
        private readonly IKernel _container;

        public YottoBus(IKernel container)
        {
            _container = container;
        }

        public List<IBusLogger> Loggers { get; } = new List<IBusLogger>()
        {
            new ConsoleLogger()
        };

        public IPeer CreatePeer(PeerConfiguration configuration)
        {
            var conf = new Ninject.Parameters.ConstructorArgument("configuration", configuration);

            return _container.Get<IPeer>(conf);
        }
    }
}
