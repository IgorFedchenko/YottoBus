using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Yotto.ServiceBus.Tests
{
    class TestsBase
    {
        protected void AwaitAssert(TimeSpan awaitTime, Action expression)
        {
            Task deadline = Task.Delay(awaitTime);
            bool passed = false;
            string errorMessage = string.Empty;
            while (!passed && !deadline.IsCompleted)
            {
                try
                {
                    expression();
                    passed = true;
                    break;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.ToString();
                }
            }

            if (!passed)
                Assert.Fail(errorMessage);
        }
    }
}
