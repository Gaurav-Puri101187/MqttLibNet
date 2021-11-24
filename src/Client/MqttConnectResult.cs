using MqttLibNet.Packets;
using System.Collections.Generic;

namespace MqttLibNet.Client
{
    /// <summary>
    /// Result for the connection request. 
    /// </summary>
    public class MqttConnectResult
    {
        /// <summary>
        /// Tells wether Session was present or not on the broker.
        /// </summary>
        public bool SessionPresent { get; set; }
        /// <summary>
        /// Response message for the connect request.
        /// </summary>
        public ConnectReturnCode ConnectReturnCode { get; set; }
        /// <summary>
        /// Topic config after subscribtion is accepted by broker.
        /// </summary>
        public IEnumerable<MqttTopicConfiguration> TopicConfiguration { get; set; }
    }
}
