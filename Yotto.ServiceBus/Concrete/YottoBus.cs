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
    /// <summary>
    /// Implements service bus
    /// </summary>
    /// <seealso cref="Yotto.ServiceBus.Abstract.IServiceBus" />
    public class YottoBus : IServiceBus
    {
        private readonly IKernel _container;
        private IDeliveryStrategy _deliveryStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="YottoBus"/> class.
        /// </summary>
        /// <param name="container">The IoC container.</param>
        public YottoBus(IKernel container)
        {
            _container = container;

            Loggers = new List<IBusLogger>()
            {
                new ConsoleLogger(),
            };

            DeliveryStrategy = new SequentialDeliveryStrategy(this);
        }

        /// <summary>
        /// Gets or sets the delivery strategy to be used to deliver message to their subscrubers.
        /// </summary>
        /// <value>
        /// The delivery strategy.
        /// </value>
        /// <exception cref="System.ArgumentNullException">DeliveryStrategy</exception>
        /// <remarks>
        /// By default, sequensial strategy is used <see cref="SequentialDeliveryStrategy" />
        /// </remarks>
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

        /// <summary>
        /// Gives access to bus loggers collection - feel free to add your custom loggers in it
        /// </summary>
        /// <value>
        /// The loggers to be used inside the bus.
        /// </value>
        /// <remarks>
        /// By default, it includes console logger <see cref="ConsoleLogger" />
        /// </remarks>
        public List<IBusLogger> Loggers { get; }

        /// <summary>
        /// Creates the peer on the bus.
        /// </summary>
        /// <param name="configuration">The peer configuration.</param>
        /// <returns>
        /// Created peer
        /// </returns>
        public IPeer CreatePeer(PeerConfiguration configuration)
        {
            var conf = new Ninject.Parameters.ConstructorArgument("configuration", configuration);

            return _container.Get<IPeer>(conf);
        }
    }
}
