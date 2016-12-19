using Yotto.ServiceBus.Abstract;

namespace Yotto.ServiceBus.Concrete
{
    class EndpointRangeParser : IEndpointsParser
    {
        public string[] Parse(string endpointsRange)
        {
            return new string[0];
        }
    }
}
