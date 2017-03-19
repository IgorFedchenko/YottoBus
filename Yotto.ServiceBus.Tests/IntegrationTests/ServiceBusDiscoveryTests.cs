using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Yotto.ServiceBus.Concrete;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;
using Yotto.ServiceBus.Extensions;

namespace Yotto.ServiceBus.Tests.IntegrationTests
{
    [TestFixture]
    class ServiceBusDiscoveryTests : TestsBase
    {
        [Test]
        public void ShouldDiscoverEachOther()
        {
            var bus = new YottoBusFactory().Create();

            using (var peer1 = bus.CreatePeer())
            using (var peer2 = bus.CreatePeer())
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
        public void PeersInDifferentContextsDoNotSeeEachOther()
        {
            var bus = new YottoBusFactory().Create();

            using (var peer1 = bus.CreatePeer("contextOne"))
            using (var peer2 = bus.CreatePeer("contextTwo"))
            {
                peer1.Connect();
                peer2.Connect();

                Thread.Sleep(5000);

                CollectionAssert.AreEquivalent(new[] { peer1.Identity }, peer1.GetConnectedPeers());
                CollectionAssert.AreEquivalent(new[] { peer2.Identity }, peer2.GetConnectedPeers());
            }
        }
        

        [Test]
        public void ShouldSetIsConnectedProperly()
        {
            var bus = new YottoBusFactory().Create();

            using (var peer1 = bus.CreatePeer())
            {
                peer1.Connect();

                Assert.True(peer1.IsConnected);

                peer1.Disconnect();

                Assert.False(peer1.IsConnected);
            }
        }

        [Test]
        public void ShouldDiscoverWithMetadata()
        {
            var bus = new YottoBusFactory().Create();

            var conf = new PeerConfiguration(string.Empty, new Dictionary<string, string>()
            {
                ["key"] = "value"
            });

            using (var peer1 = bus.CreatePeer(conf))
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
            var bus = new YottoBusFactory().Create();

            using (var peer1 = bus.CreatePeer())
            using (var peer2 = bus.CreatePeer())
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

        /// <summary>
        /// This test is running in debug mode so that we can examine consumed memory with VisualStudio tools
        /// </summary>
        // [Test]
        public void MemoryConsumptionTest()
        {
            var bus = new YottoBusFactory().Create();

            using (var peer1 = bus.CreatePeer())
            using (var peer2 = bus.CreatePeer())
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
