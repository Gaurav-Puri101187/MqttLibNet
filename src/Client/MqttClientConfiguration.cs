using System.Collections.Generic;

namespace MqttLibNet.Client
{
    /// <summary>
    /// This is the basic configuration required for connecting to Mqtt broker
    /// </summary>
    public class MqttClientConfiguration
    {
        /// <summary>
        /// The Protocol Name is a UTF-8 encoded string that represents the protocol name “MQTT”
        /// </summary>
        public string ProtocolName { get; private set; } = "MQTT";
        /// <summary>
        /// The 8 bit unsigned value that represents the revision level of the protocol used by the Client.
        /// </summary>
        public short ProtocolLevel { get; set; }
        /// <summary>
        /// TODO for Auth purpose
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// TODO for Auth purpose
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Expressed as a 16-bit word, it is the maximum time interval that is permitted to elapse between 
        /// the point at which the Client finishes transmitting one Control Packet 
        /// and the point it starts sending the next. 
        /// It is the responsibility of the Client to ensure that the interval between 
        /// Control Packets being sent does not exceed the Keep Alive value. In the absence of sending any other Control Packets, 
        /// the Client MUST send a PINGREQ Packet
        /// </summary>
        public short KeepAliveMs { get; set; }
        /// <summary>
        /// The Client Identifier (ClientId) identifies the Client to the Server. 
        /// </summary>
        public string ClientIdentifier { get; set; }
        /// <summary>
        /// TODO Will message handling.
        /// </summary>
        public string WillTopic { get; set; }
        /// <summary>
        /// TODO Will message handling.
        /// </summary>
        public string WillMessage { get; set; }
        /// <summary>
        /// TODO Will message handling.
        /// </summary>
        public short WillQos { get; set; }
        /// <summary>
        /// TODO Will message handling.
        /// </summary>
        public bool WillRetain { get; set; }
        /// <summary>
        /// This bit specifies the handling of the Session state. 
        /// </summary>
        public bool CleanSession { get; set; }
        /// <summary>
        /// Required topic config.
        /// </summary>
        public IEnumerable<MqttTopicConfiguration> TopicConfiguration { get; set; }
    }
}
