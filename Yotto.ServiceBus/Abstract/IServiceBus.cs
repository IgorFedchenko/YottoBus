using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Concrete.DeliveryStrategies;
using Yotto.ServiceBus.Concrete.Loggers;
using Yotto.ServiceBus.Configuration;

namespace Yotto.ServiceBus.Abstract
{
    /// <summary>
    /// Describes the service bus
    /// </summary>
    public interface IServiceBus
    {
        /// <summary>
        /// Gives access to bus loggers collection - feel free to add your custom loggers in it
        /// </summary>
        /// <value>
        /// The loggers to be used inside the bus.
        /// </value>
        /// <remarks>By default, it includes console logger <see cref="ConsoleLogger"/></remarks>
        List<IBusLogger> Loggers { get; }

        /// <summary>
        /// Gets or sets the delivery strategy to be used to deliver message to their subscrubers.
        /// </summary>
        /// <value>
        /// The delivery strategy.
        /// </value>
        /// <remarks>By default, sequensial strategy is used <see cref="SequentialDeliveryStrategy"/></remarks>
        IDeliveryStrategy DeliveryStrategy { get; set; }

        /// <summary>
        /// Creates the peer on the bus.
        /// </summary>
        /// <param name="configuration">The peer configuration.</param>
        /// <returns>Created peer</returns>
        IPeer CreatePeer(PeerConfiguration configuration);
    }
}
