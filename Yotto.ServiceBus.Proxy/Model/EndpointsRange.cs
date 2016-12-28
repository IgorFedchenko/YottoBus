using System.Collections.Generic;
using System.Net;
using Yotto.ServiceBus.Proxy.Helpers;

namespace Yotto.ServiceBus.Proxy.Model
{
    public class EndpointsRange
    {
        public EndpointsRange(string endpointsPattern) : this(EndpointsRangeParser.Parse(endpointsPattern))
        {
        }

        public EndpointsRange(List<IPEndPoint> endpoints)
        {
            All = endpoints;
        }

        public List<IPEndPoint> All { get; } 
    }
}
