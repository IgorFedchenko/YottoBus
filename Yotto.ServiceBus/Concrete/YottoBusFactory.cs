using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Configuration;

namespace Yotto.ServiceBus.Concrete
{
    public static class YottoBusFactory
    {
        private static IKernel _container;

        static YottoBusFactory()
        {
            RegisterDependencies();
        }

        public static IServiceBus Create()
        {
            return _container.Get<IServiceBus>();
        }

        private static void RegisterDependencies()
        {
            _container = new StandardKernel();
            _container.Bind<IServiceBus>().To<YottoBus>().InSingletonScope();
            _container.Bind<IPeer>().To<Peer>();
            _container.Bind<ISubscriber>().To<Subscriber>();
            _container.Bind<IPublisher>().To<Publisher>();
            _container.Bind<IConnectionTracker>().To<ConnectionTracker>();
        }
    }
}
