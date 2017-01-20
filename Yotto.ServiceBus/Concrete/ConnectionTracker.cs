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
    /// <summary>
    /// Tracks other peers with heartbeats and sends self heartbeat
    /// </summary>
    /// <seealso cref="Yotto.ServiceBus.Abstract.IConnectionTracker" />
    class ConnectionTracker : IConnectionTracker
    {
        const int HeatbeatDeadline = 5000; // How long to wait before mark peer disconnected

        private IPublisher _publisher;
        private ISubscriber _subscriber;
        private CancellationTokenSource _cancel;
        private PeerIdentity _selfIdentity;
        private readonly ConcurrentDictionary<PeerIdentity, Deadline> _peerHeartbeatDeadlines = new ConcurrentDictionary<PeerIdentity, Deadline>();

        /// <summary>
        /// Occurs when new peer connected.
        /// </summary>
        public event Action<PeerIdentity> PeerConnected;

        /// <summary>
        /// Occurs when new peer disconnected.
        /// </summary>
        public event Action<PeerIdentity> PeerDisconnected;

        /// <summary>
        /// Starts peers tracking.
        /// </summary>
        /// <param name="publisher">The publisher to use.</param>
        /// <param name="subscriber">The subscriber to use.</param>
        /// <param name="selfIdentity">The self identity of peer.</param>
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

        /// <summary>
        /// Stops tracking.
        /// </summary>
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

        /// <summary>
        /// Handles message if it contains heartbeat.
        /// </summary>
        /// <param name="msg">The MSG.</param>
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
