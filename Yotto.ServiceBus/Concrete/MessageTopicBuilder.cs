using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Concrete
{
    class MessageTopicBuilder
    {
        public string GetMessageTag(string context, Type messageType)
        {
            return $"{context};{messageType.AssemblyQualifiedName}";
        }

        public string GetMessageTag(string context, Guid id)
        {
            return $"{context};{id}";
        }
    }
}
