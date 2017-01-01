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
            var bus = YottoBus.Create();

            using (var peer1 = bus.CreatePeer(new PeerConfiguration() { Metadata = new PeerMetadata(new Dictionary<string, string>() { ["key"] = "value" })}))
            using (var peer2 = bus.CreatePeer(new PeerConfiguration()))
            {
                peer1.Connect();
                peer2.Connect();

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    Assert.AreEqual(peer1.GetConnectedPeers().First().Id, peer2.Identity.Id);
                    Assert.True(peer1.GetConnectedPeers().First().Metadata.Has("key"));
                    Assert.AreEqual(peer1.GetConnectedPeers().First().Metadata.Get("key"), "value");
                });
            }
        }

        [Test]
        public void ShouldDiscoverDisconnect()
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

                peer2.Disconnect();

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    CollectionAssert.IsEmpty(peer1.GetConnectedPeers());
                });
            }
        }

        // [Test]
        public void MemoryConsumptionTest()
        {
            var bus = YottoBus.Create();

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
