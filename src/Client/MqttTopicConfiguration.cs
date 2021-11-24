using MqttLibNet.Packets;

namespace MqttLibNet.Client
{
    /// <summary>
    /// Configuration for the topics to subscribe to.
    /// </summary>
    public class MqttTopicConfiguration
    {
        public MqttTopicConfiguration(string name, QosLevel qosLevel)
        {
            Name = name;
            Level = qosLevel;
        }
        /// <summary>
        /// Name of the topic to subscribe to
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Desired level for subscription.
        /// </summary>
        public QosLevel Level { get; set; }
    }
}
