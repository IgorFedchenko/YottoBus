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
            var bus = YottoBus.Create();

            using (var peer1 = bus.CreatePeer(new PeerConfiguration()))
            using (var peer2 = bus.CreatePeer(new PeerConfiguration()))
            {
                peer1.Connect();
                peer2.Connect();

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    CollectionAssert.AreEquivalent(new[] { peer2.Identity }, peer1.GetConnectedPeers());
                    CollectionAssert.AreEquivalent(new[] { peer1.Identity }, peer2.GetConnectedPeers());
                });
            }
        }

        [Test]
        public void ShouldDiscoverWithInnerData()
        {
            var endpoint1 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            var endpoint2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);

            using (var peer1 = YottoBus.CreatePeer(endpoint1, new TagsList("a", "b")))
            using (var peer2 = YottoBus.CreatePeer(endpoint2, new TagsList("c", "d")))
            {
                peer1.Connect(new EndpointsRange("127.0.0.1:8081"));
                peer2.Connect(new EndpointsRange("127.0.0.1:8080"));

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    Assert.AreEqual(peer1.GetConnectedPeers().First().Endpoint, peer2.Identity.Endpoint);
                    CollectionAssert.AreEqual(peer1.GetConnectedPeers().First().Tags.AllTags, peer2.Identity.Tags.AllTags);
                });
            }
        }

        [Test]
        public void ShouldDiscoverDisconnect()
        {
            var endpoint1 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            var endpoint2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);

            using (var peer1 = YottoBus.CreatePeer(endpoint1, TagsList.Empty))
            using (var peer2 = YottoBus.CreatePeer(endpoint2, TagsList.Empty))
            {
                peer1.Connect(new EndpointsRange("127.0.0.1:8081"));
                peer2.Connect(new EndpointsRange("127.0.0.1:8080"));

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    CollectionAssert.AreEquivalent(new[] { peer2.Identity }, peer1.GetConnectedPeers());
                    CollectionAssert.AreEquivalent(new[] { peer1.Identity }, peer2.GetConnectedPeers());
                });

                peer2.Disconnect();

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    CollectionAssert.IsEmpty(peer1.GetConnectedPeers());
                });
            }
        }

        [Test]
        public void MemoryConsumptionTest()
        {
            var endpoint1 = new IPEndPoint(IPAddress.Parse("10.5.5.3"), 8080);
            var endpoint2 = new IPEndPoint(IPAddress.Parse("10.5.5.3"), 8081);

            using (var peer1 = YottoBus.CreatePeer(endpoint1, TagsList.Empty))
            using (var peer2 = YottoBus.CreatePeer(endpoint2, TagsList.Empty))
            {
                peer1.Connect(new EndpointsRange("10.5.5.[1-254]:[8000-8020]"));
                peer2.Connect(new EndpointsRange("10.5.5.[1-254]:[8000-8020]"));

                while (true)
                {
                    
                }
            }
        }
    }
}
