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
using Yotto.ServiceBus.Model;

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

        public void MultipleSubscriptionsTest()
        {
            
        }
    }
}
