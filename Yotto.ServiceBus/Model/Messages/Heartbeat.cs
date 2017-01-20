namespace Yotto.ServiceBus.Model.Messages
{
    /// <summary>
    /// Sent and received by all peers to indicate active connections
    /// </summary>
    class Heartbeat
    {
        /// <summary>
        /// Identity of sender
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        public PeerIdentity Identity { get; set; }
    }
}
