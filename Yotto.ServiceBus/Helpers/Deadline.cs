using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Helpers
{
    /// <summary>
    /// Incapsulates the deadline logic
    /// </summary>
    class Deadline
    {
        private readonly DateTime _finishDateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Deadline"/> class.
        /// </summary>
        /// <param name="duration">The duration of this deadline, from now.</param>
        public Deadline(TimeSpan duration)
        {
            _finishDateTime = DateTime.Now + duration;
        }

        /// <summary>
        /// Gets a value indicating whether this deadline is expired.
        /// </summary>
        /// <value>
        /// <c>true</c> if deadline instance is expired; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpired => DateTime.Now > _finishDateTime;
    }
}
