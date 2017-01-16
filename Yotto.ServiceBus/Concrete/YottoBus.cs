using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Activation;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Concrete.DeliveryStrategies;
using Yotto.ServiceBus.Concrete.Loggers;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Concrete
{
    public class YottoBus : IServiceBus
    {
        private readonly IKernel _container;
        private IDeliveryStrategy _deliveryStrategy;

        public YottoBus(IKernel container)
        {
            _container = container;

            Loggers = new List<IBusLogger>()
            {
                new ConsoleLogger(),
            };
            DeliveryStrategy = new SequentialDeliveryStrategy(Loggers);
            // DeliveryStrategy = new ParallelDeliveryStrategy(Loggers);
            // DeliveryStrategy = new ActorDeliveryStrategy(Loggers);
        }

        public IDeliveryStrategy DeliveryStrategy
        {
            get
            {
                return _deliveryStrategy;
            }
            set
            {
                if (value == null)
                    throw  new ArgumentNullException(nameof(DeliveryStrategy));

                _deliveryStrategy = value;
            }
        }

        public List<IBusLogger> Loggers { get; }

        public IPeer CreatePeer(PeerConfiguration configuration)
        {
            var conf = new Ninject.Parameters.ConstructorArgument("configuration", configuration);

            return _container.Get<IPeer>(conf);
        }
    }
}
