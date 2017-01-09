using System;
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
        private readonly Dictionary<PeerIdentity, Deadline> _peerHeartbeatDeadlines = new Dictionary<PeerIdentity, Deadline>();

        public event Action<PeerIdentity> PeerConnected;
        public event Action<PeerIdentity> PeerDisconnected;

        public void Start(IPublisher publisher, ISubscriber subscriber, PeerIdentity selfIdentity)
        {
            _publisher = publisher;
            _subscriber = subscriber;
            _selfIdentity = selfIdentity;

            _subscriber.SubscribeTo<Heartbeat>();

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
                await Task.Delay(TimeSpan.FromSeconds(1));

                _publisher.Publish(new Heartbeat() { Identity = _selfIdentity });
            }
        }

        private async void StartCheckPeerHearbeats()
        {
            while (!_cancel.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                foreach (var peerHeartbeatDeadline in _peerHeartbeatDeadlines.Where(peerHeartbeatDeadline => peerHeartbeatDeadline.Value.IsExpired).ToArray())
                {
                    _peerHeartbeatDeadlines.Remove(peerHeartbeatDeadline.Key);

                    foreach (var handler in PeerDisconnected?.GetInvocationList().ToArray() ?? new Delegate[0])
                    {
                        ((Action<PeerIdentity>)handler)?.BeginInvoke(peerHeartbeatDeadline.Key, null, null);
                    }
                }
            }
        }

        private void HandleIfHeartbeat(Message msg)
        {
            var heartbeat = msg.Content as Heartbeat;
            if (heartbeat != null)
            {
                if (!_peerHeartbeatDeadlines.ContainsKey(heartbeat.Identity))
                {
                    _peerHeartbeatDeadlines.Add(heartbeat.Identity, new Deadline(TimeSpan.FromMilliseconds(HeatbeatDeadline)));

                    foreach (var handler in PeerConnected?.GetInvocationList().ToArray() ?? new Delegate[0])
                    {
                        ((Action<PeerIdentity>) handler)?.BeginInvoke(heartbeat.Identity, null, null);
                    }
                }
                else
                {
                    _peerHeartbeatDeadlines[heartbeat.Identity] = new Deadline(TimeSpan.FromMilliseconds(HeatbeatDeadline));
                }
            }
        }
    }
}
