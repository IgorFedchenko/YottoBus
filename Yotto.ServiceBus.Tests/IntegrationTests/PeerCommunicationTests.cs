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
            public TMessage ReceivedMessage { get; private set; }
            public PeerIdentity MessageSender { get; private set; }

            public void Handle(TMessage @event, PeerIdentity sender)
            {
                ReceivedMessage = @event;
                MessageSender = sender;
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
                    Assert.AreEqual(message, subscriber.ReceivedMessage);
                    Assert.AreEqual(peer1.Identity, subscriber.MessageSender);
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
                    Assert.True(subscriberForConnected.ReceivedMessage != null && subscriberForConnected.ReceivedMessage.Identity.Equals(peer2.Identity));
                });

                peer2.Disconnect();

                AwaitAssert(TimeSpan.FromSeconds(10), () =>
                {
                    Assert.True(subscriberForDisconnected.ReceivedMessage != null && subscriberForDisconnected.ReceivedMessage.Identity.Equals(peer2.Identity));
                });
            }
        }

        public void MultipleSubscriptionsTest()
        {
            
        }
    }
}
