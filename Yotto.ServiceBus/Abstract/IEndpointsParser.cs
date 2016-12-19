using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Abstract
{
    interface IEndpointsParser
    {
        string[] Parse(string endpointsString);
    }
}
