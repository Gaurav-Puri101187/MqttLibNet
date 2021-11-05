using System.Linq;
using System.Text;
using MqttLibNet.Utils;

namespace MqttLibNet.Packets
{
    public class PublishQos1 : Publish
    {
        public override PublishData Deserialize(byte[] packetBytes)
        {
             PublishData publishData = new PublishData();
            var topicNameLength = packetBytes.Take(2).ToArray().GetMqttInt16();
            var topicNameBuffer = packetBytes.ToList().GetRange(2, topicNameLength).ToArray();
            publishData.TopicName = Encoding.UTF8.GetString(topicNameBuffer);
            publishData.PacketIdentifier = (short)packetBytes.ToList().GetRange(topicNameLength +2, 2).ToArray().GetMqttInt16();
            var payloadBuffer = packetBytes.ToList().GetRange(topicNameLength + 4, (packetBytes.Count() - (topicNameLength + 4))).ToArray();
            publishData.Message = Encoding.UTF8.GetString(payloadBuffer);
            return publishData;
        }
    }
}