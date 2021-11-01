using MqttLibNet.Utils;
using System.Linq;
using System.Text;

namespace MqttLibNet.Packets
{
    public class PublishQos0 : Publish
    {
        public override PublishData Deserialize(byte[] packetBytes)
        {
            PublishData publishData = new PublishData();
            var topicNameLength = packetBytes.Take(2).ToArray().GetMqttInt16();
            var topicNameBuffer = packetBytes.ToList().GetRange(2, topicNameLength).ToArray();
            publishData.TopicName = Encoding.UTF8.GetString(topicNameBuffer);
            var payloadBuffer = packetBytes.ToList().GetRange(topicNameLength + 2, (packetBytes.Count() - (topicNameLength + 2))).ToArray();
            publishData.Message = Encoding.UTF8.GetString(payloadBuffer);
            return publishData;
        }
    }
}
