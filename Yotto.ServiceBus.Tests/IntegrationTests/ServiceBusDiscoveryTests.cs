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
            var bus = YottoBusFactory.Create();

            using (var peer1 = bus.CreatePeer(new PeerConfiguration()))
            using (var peer2 = bus.CreatePeer(new PeerConfiguration()))
            {
                peer1.Connect();
                peer2.Connect();

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    CollectionAssert.AreEquivalent(new[] { peer1.Identity, peer2.Identity }, peer1.GetConnectedPeers());
                    CollectionAssert.AreEquivalent(new[] { peer1.Identity, peer2.Identity }, peer2.GetConnectedPeers());
                });
            }
        }

        [Test]
        public void ShouldDiscoverWithInnerData()
        {
            var bus = YottoBusFactory.Create();

            using (var peer1= bus.CreatePeer(new PeerConfiguration() { Metadata = new PeerMetadata(new Dictionary<string, string>() { ["key"] = "value" }) }))
            {
                peer1.Connect();

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    Assert.True(peer1.GetConnectedPeers().Any());
                    Assert.True(peer1.GetConnectedPeers().First().Metadata.Has("key"));
                    Assert.AreEqual(peer1.GetConnectedPeers().First().Metadata.Get("key"), "value");
                });
            }
        }

        [Test]
        public void ShouldDiscoverDisconnect()
        {
            var bus = YottoBusFactory.Create();

            using (var peer1 = bus.CreatePeer(new PeerConfiguration()))
            using (var peer2 = bus.CreatePeer(new PeerConfiguration()))
            {
                peer1.Connect();
                peer2.Connect();

                Console.WriteLine(DateTime.Now + "Started");

                AwaitAssert(TimeSpan.FromSeconds(50), () =>
                {
                    CollectionAssert.Contains(peer1.GetConnectedPeers(), peer2.Identity);
                });

                Console.WriteLine(DateTime.Now + "Connected");

                peer2.Disconnect();

                AwaitAssert(TimeSpan.FromSeconds(50), () =>
                {
                    CollectionAssert.DoesNotContain(peer1.GetConnectedPeers(), peer2.Identity);
                });

                Console.WriteLine(DateTime.Now + "Disconnected");
            }
        }

        // [Test]
        public void MemoryConsumptionTest()
        {
            var bus = YottoBusFactory.Create();

            using (var peer1 = bus.CreatePeer(new PeerConfiguration()))
            using (var peer2 = bus.CreatePeer(new PeerConfiguration()))
            {
                peer1.Connect();
                peer2.Connect();

                while (true)
                {
                    
                }
            }
        }
    }
}
