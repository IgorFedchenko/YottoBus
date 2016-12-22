using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.Framework;
using Yotto.ServiceBus.Concrete;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;

namespace Yotto.ServiceBus.Tests.IntegrationTests
{
    [TestFixture]
    class ServiceBusDiscoveryTests : TestsBase
    {
        [Test]
        public void ShouldDiscoverEachOther()
        {
            var endpoint1 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            var endpoint2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);

            using (var peer1 = BusConnectionFactory.CreateClient(endpoint1, TagsList.Empty))
            using (var peer2 = BusConnectionFactory.CreateClient(endpoint2, TagsList.Empty))
            {
                peer1.Connect("127.0.0.1:8081");
                peer2.Connect("127.0.0.1:8080");

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    CollectionAssert.AreEquivalent(new[] { peer2.Self }, peer1.GetPeers());
                    CollectionAssert.AreEquivalent(new[] { peer1.Self }, peer2.GetPeers());
                });
            }
        }

        [Test]
        public void ShouldDiscoverWithInnerData()
        {
            var endpoint1 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            var endpoint2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);

            using (var peer1 = BusConnectionFactory.CreateClient(endpoint1, new TagsList("a", "b")))
            using (var peer2 = BusConnectionFactory.CreateClient(endpoint2, new TagsList("c", "d")))
            {
                peer1.Connect("127.0.0.1:8081");
                peer2.Connect("127.0.0.1:8080");

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    Assert.AreEqual(peer1.GetPeers().First().Endpoint, peer2.Self.Endpoint);
                    CollectionAssert.AreEqual(peer1.GetPeers().First().Tags.AllTags, peer2.Self.Tags.AllTags);
                });
            }
        }
    }
}
