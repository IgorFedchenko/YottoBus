using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Concrete.DeliveryStrategies;
using Yotto.ServiceBus.Concrete.Loggers;
using Yotto.ServiceBus.Configuration;

namespace Yotto.ServiceBus.Concrete
{
    /// <summary>
    /// Factory for creating default configured bus
    /// </summary>
    public class YottoBusFactory
    {
        protected virtual IKernel Container { get; } = new StandardKernel();

        public YottoBusFactory()
        {
            RegisterDependencies();
        }

        public YottoBusFactory UsingDeliveryStrategy<TDelivertStranegy>() where TDelivertStranegy : DeliveryStrategyBase
        {
            Container.Rebind<DeliveryStrategyBase>().To<TDelivertStranegy>();

            return this;
        }

        public YottoBusFactory WithLogger<TLogger>() where TLogger : IBusLogger
        {
            Container.Bind<IBusLogger>().To<TLogger>();

            return this;
        }

        /// <summary>
        /// Creates the bus instance.
        /// </summary>
        /// <returns></returns>
        public IServiceBus Create()
        {
            return Container.Get<IServiceBus>();
        }

        private void RegisterDependencies()
        {
            Container.Bind<IServiceBus>().To<YottoBus>().InSingletonScope();
            Container.Bind<IPeer>().To<Peer>();
            Container.Bind<ISubscriber>().To<Subscriber>();
            Container.Bind<IPublisher>().To<Publisher>();
            Container.Bind<IBusLogger>().To<ConsoleLogger>();
            Container.Bind<IConnectionTracker>().To<ConnectionTracker>();
            Container.Bind<DeliveryStrategyBase>().To<SequentialDeliveryStrategy>();
        }
    }
}
