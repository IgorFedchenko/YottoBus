using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Concrete;
using Yotto.ServiceBus.Concrete.DeliveryStrategies;
using Yotto.ServiceBus.Concrete.Loggers;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Model;
using Yotto.ServiceBus.Model.Messages;
using Yotto.ServiceBus.Extensions;

namespace Yotto.ServiceBus.Tests.IntegrationTests
{
    [TestFixture]
    class PeerCommunicationTests : TestsBase
    {
        class SubscriberFor<TMessage> : IMessageHandler<TMessage>
        {
            public List<object> ReceivedMessages { get; } = new List<object>();
            public List<PeerIdentity> MessageSenders { get; } = new List<PeerIdentity>();

            public void Handle(TMessage @event, PeerIdentity sender)
            {
                ReceivedMessages.Add(@event);
                MessageSenders.Add(sender);
            }
        }

        class SubscriberForStringAndInt : SubscriberFor<string>, IMessageHandler<int>
        {
            public void Handle(int @event, PeerIdentity sender)
            {
                ReceivedMessages.Add(@event);
                MessageSenders.Add(sender);
            }
        }

        [Test]
        public void PublishSubscribeTest()
        {
            var bus = new YottoBusFactory().Create();

            using (var peer1 = bus.CreatePeer())
            {
                peer1.Connect();

                var subscriber = new SubscriberFor<string>();
                peer1.Register(subscriber);

                Thread.Sleep(50);

                var message = "Hello";
                peer1.Publish(message);

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    CollectionAssert.Contains(subscriber.ReceivedMessages, message);
                    CollectionAssert.Contains(subscriber.MessageSenders, peer1.Identity);
                });
            }
        }

        [Test]
        public void SubscribeToPeerConnectedOrDisconnectedTest()
        {
            var bus = new YottoBusFactory().Create();

            using (var peer1 = bus.CreatePeer())
            using (var peer2 = bus.CreatePeer())
            {
                peer1.Connect();

                var subscriberForConnected = new SubscriberFor<PeerConnected>();
                var subscriberForDisconnected = new SubscriberFor<PeerDisconnected>();
                peer1.Register(subscriberForConnected);
                peer1.Register(subscriberForDisconnected);

                Thread.Sleep(50);

                peer2.Connect();

                AwaitAssert(TimeSpan.FromSeconds(10), () =>
                {
                    Assert.True(subscriberForConnected.ReceivedMessages.Any(m => m is PeerConnected && ((PeerConnected)m).Identity.Equals(peer2.Identity)));
                });

                peer2.Disconnect();

                AwaitAssert(TimeSpan.FromSeconds(10), () =>
                {
                    Assert.True(subscriberForDisconnected.ReceivedMessages.Any(m => m is PeerDisconnected && ((PeerDisconnected)m).Identity.Equals(peer2.Identity)));
                });
            }
        }

        [Test]
        public void MultipleSubscriptionsTest()
        {
            var bus = new YottoBusFactory().Create();

            using (var peer1 = bus.CreatePeer())
            {
                peer1.Connect();

                var subscriber = new SubscriberForStringAndInt();
                peer1.Register(subscriber);

                Thread.Sleep(50);

                var stringMessage = "Hello";
                var intMessage = 5;
                peer1.Publish(stringMessage);
                peer1.Publish(intMessage);

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    CollectionAssert.Contains(subscriber.ReceivedMessages, stringMessage);
                    CollectionAssert.Contains(subscriber.ReceivedMessages, intMessage);
                });
            }
        }

        [Test]
        public void SendDedicatedMessageTest()
        {
            var bus = new YottoBusFactory().Create();

            using (var peer1 = bus.CreatePeer())
            using (var peer2 = bus.CreatePeer())
            {
                peer1.Connect();
                peer2.Connect();

                var subscriberToReceive = new SubscriberFor<string>();
                var subscriberToSkip = new SubscriberFor<string>();
                peer1.Register(subscriberToReceive);
                peer2.Register(subscriberToSkip);

                Thread.Sleep(50);

                var message = "Hello";
                peer2.Send(message, peer1.Identity);

                AwaitAssert(TimeSpan.FromSeconds(5), () =>
                {
                    CollectionAssert.Contains(subscriberToReceive.ReceivedMessages, message);
                });

                Thread.Sleep(1000);

                CollectionAssert.IsEmpty(subscriberToSkip.ReceivedMessages);
            }
        }

        [Test]
        public void PeersInDifferentContextDoNotReceiveSubscribedMessages()
        {
            var bus = new YottoBusFactory().Create();

            using (var peer1 = bus.CreatePeer("contextOne"))
            using (var peer2 = bus.CreatePeer("contextTwo"))
            {
                peer1.Connect();
                peer2.Connect();

                var subscriber = new SubscriberFor<string>();
                peer1.Register(subscriber);

                Thread.Sleep(50);

                var message = "Hello";
                peer2.Publish(message);

                Thread.Sleep(5000);

                CollectionAssert.DoesNotContain(subscriber.ReceivedMessages, message);
                CollectionAssert.DoesNotContain(subscriber.MessageSenders, peer1.Identity);
            }
        }
    }
}
