using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yotto.ServiceBus.Abstract;
using Yotto.ServiceBus.Helpers;
using Yotto.ServiceBus.Model;
using Yotto.ServiceBus.Model.Messages;

namespace Yotto.ServiceBus.Concrete
{
    class ConnectionTracker : IConnectionTracker
    {
        const int HeatbeatDeadline = 5000; 

        private IPublisher _publisher;
        private ISubscriber _subscriber;
        private CancellationTokenSource _cancel;
        private PeerIdentity _selfIdentity;
        private readonly ConcurrentDictionary<PeerIdentity, Deadline> _peerHeartbeatDeadlines = new ConcurrentDictionary<PeerIdentity, Deadline>();

        public event Action<PeerIdentity> PeerConnected;
        public event Action<PeerIdentity> PeerDisconnected;

        public void Start(IPublisher publisher, ISubscriber subscriber, PeerIdentity selfIdentity)
        {
            _publisher = publisher;
            _subscriber = subscriber;
            _selfIdentity = selfIdentity;

            _subscriber.SubscribeTo(typeof(Heartbeat));

            _subscriber.MessageReceived += HandleIfHeartbeat;

            _cancel = new CancellationTokenSource();
            StartSendingHeartbeats();
            StartCheckPeerHearbeats();
        }

        public void Stop()
        {
            _subscriber.MessageReceived -= HandleIfHeartbeat;
            _cancel?.Cancel();
        }

        private async void StartSendingHeartbeats()
        {
            while (!_cancel.IsCancellationRequested)
            {
                _publisher.Publish(new Heartbeat() { Identity = _selfIdentity });

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private async void StartCheckPeerHearbeats()
        {
            while (!_cancel.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                foreach (var peerHeartbeatDeadline in _peerHeartbeatDeadlines.Where(peerHeartbeatDeadline => peerHeartbeatDeadline.Value.IsExpired).ToArray())
                {
                    Deadline tmp;
                    _peerHeartbeatDeadlines.TryRemove(peerHeartbeatDeadline.Key, out tmp);

                    foreach (var handler in PeerDisconnected?.GetInvocationList().ToArray() ?? new Delegate[0])
                    {
                        ((Action<PeerIdentity>)handler)?.Invoke(peerHeartbeatDeadline.Key);
                    }
                }
            }
        }

        private void HandleIfHeartbeat(Message msg)
        {
            var heartbeat = msg.Content as Heartbeat;
            if (heartbeat != null)
            {
                _peerHeartbeatDeadlines.AddOrUpdate(heartbeat.Identity,
                    identity =>
                    {
                        var deadline = new Deadline(TimeSpan.FromMilliseconds(HeatbeatDeadline));

                        foreach (var handler in PeerConnected?.GetInvocationList().ToArray() ?? new Delegate[0])
                        {
                            ((Action<PeerIdentity>)handler)?.Invoke(heartbeat.Identity);
                        }

                        return deadline;
                    },
                    (identity, deadline) => new Deadline(TimeSpan.FromMilliseconds(HeatbeatDeadline)));
            }
        }
    }
}
