using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Helpers
{
    class Deadline
    {
        private readonly DateTime _finishDateTime;

        public Deadline(TimeSpan duration)
        {
            _finishDateTime = DateTime.Now + duration;
        }

        public bool IsExpired => DateTime.Now > _finishDateTime;
    }
}
