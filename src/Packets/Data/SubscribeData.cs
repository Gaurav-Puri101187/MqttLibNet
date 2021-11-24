using MqttLibNet.Client;
using System.Collections.Generic;
using System.Linq;

namespace MqttLibNet.Packets.Data
{
    public class SubscribeData
    {
        public SubscribeData(
            IEnumerable<MqttTopicConfiguration> mqttTopicConfigurations,
            short packetIdentifier)
        {
            PacketIdentifier = packetIdentifier;
            Subscriptions = mqttTopicConfigurations.Select(_ => (_.Name, (QosLevel)_.Level));
        }
        public short PacketIdentifier { get; private set; }
        public IEnumerable<(string TopicName, QosLevel Qos)> Subscriptions { get; private set; }
    }
}
