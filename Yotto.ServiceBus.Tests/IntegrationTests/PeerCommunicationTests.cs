using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Concrete;
using Yotto.ServiceBus.Configuration;
using Yotto.ServiceBus.Loggers.Console;
using Yotto.ServiceBus.Model;
using Yotto.ServiceBus.Model.Messages;

namespace Yotto.ServiceBus.Tests.IntegrationTests
{
    [TestFixture]
    class PeerCommunicationTests : TestsBase
    {
        class SubscriberFor<TMessage> : IMessageHandler<TMessage>
        {
            public List<TMessage> ReceivedMessages { get; } = new List<TMessage>();
            public List<PeerIdentity> MessageSenders { get; } = new List<PeerIdentity>();

            public void Handle(TMessage @event, PeerIdentity sender)
            {
                ReceivedMessages.Add(@event);
                MessageSenders.Add(sender);
            }
        }

        class SubscriberForStringAndInt: IMessageHandler<string>, IMessageHandler<int>
        {
            public List<object> ReceivedMessages { get; } = new List<object>();
            public List<PeerIdentity> MessageSenders { get; } = new List<PeerIdentity>();

            public void Handle(string @event, PeerIdentity sender)
            {
                ReceivedMessages.Add(@event);
                MessageSenders.Add(sender);
            }

            public void Handle(int @event, PeerIdentity sender)
            {
                ReceivedMessages.Add(@event);
                MessageSenders.Add(sender);
            }
        }

        [Test]
        public void PublishSubscribeTest()
        {
            var bus = YottoBusFactory.Create();

            using (var peer1 = bus.CreatePeer(new PeerConfiguration()))
            {
                peer1.Connect();

                var subscriber = new SubscriberFor<string>();
                peer1.Subscribe(subscriber);

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
            var bus = YottoBusFactory.Create();
            bus.Loggers.Add(new ConsoleLogger());

            using (var peer1 = bus.CreatePeer(new PeerConfiguration()))
            using (var peer2 = bus.CreatePeer(new PeerConfiguration()))
            {
                peer1.Connect();

                var subscriberForConnected = new SubscriberFor<PeerConnected>();
                var subscriberForDisconnected = new SubscriberFor<PeerDisconnected>();
                peer1.Subscribe(subscriberForConnected);
                peer1.Subscribe(subscriberForDisconnected);

                Thread.Sleep(50);

                peer2.Connect();

                AwaitAssert(TimeSpan.FromSeconds(10), () =>
                {
                    Assert.True(subscriberForConnected.ReceivedMessages.Any(m => m.Identity.Equals(peer2.Identity)));
                });

                peer2.Disconnect();

                AwaitAssert(TimeSpan.FromSeconds(10), () =>
                {
                    Assert.True(subscriberForDisconnected.ReceivedMessages.Any(m => m.Identity.Equals(peer2.Identity)));
                });
            }
        }

        [Test]
        public void MultipleSubscriptionsTest()
        {
            var bus = YottoBusFactory.Create();

            using (var peer1 = bus.CreatePeer(new PeerConfiguration()))
            {
                peer1.Connect();

                var subscriber = new SubscriberForStringAndInt();
                peer1.Subscribe(subscriber);

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
    }
}
