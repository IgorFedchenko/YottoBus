using System.Collections.Generic;
using System.Linq;
using System.Net;
using Yotto.ServiceBus.Proxy.Helpers;

namespace Yotto.ServiceBus.Proxy.Model
{
    /// <summary>
    /// Represents endpoints range
    /// </summary>
    public class EndpointsRange
    {
        /// <summary>
        /// Generates endpoints range from endpoint pattern using <see cref="EndpointsRangeParser"/>.
        /// </summary>
        /// <param name="endpointsPattern">The endpoints pattern.</param>
        public EndpointsRange(string endpointsPattern) 
            : this(EndpointsRangeParser.Parse(endpointsPattern))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointsRange"/> class with provided endpoints list.
        /// </summary>
        /// <param name="endpoints">The endpoints.</param>
        public EndpointsRange(IEnumerable<IPEndPoint> endpoints)
        {
            All = endpoints.ToList();
        }

        /// <summary>
        /// Gets all endpoints in range.
        /// </summary>
        /// <value>
        /// All endpoints in range.
        /// </value>
        public IReadOnlyCollection<IPEndPoint> All { get; }

        /// <summary>
        /// Joins this endpoints range with specified one.
        /// </summary>
        /// <param name="range">The range to join.</param>
        /// <returns></returns>
        public EndpointsRange JoinWith(EndpointsRange range)
        {
            return new EndpointsRange(All.Concat(range.All));
        }
    }
}
