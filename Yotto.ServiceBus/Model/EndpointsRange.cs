using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yotto.ServiceBus.Helpers;

namespace Yotto.ServiceBus.Model
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
