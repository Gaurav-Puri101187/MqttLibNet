using MqttLibNet.Packets.Data;
using MqttLibNet.Utils;
using System.Collections.Generic;

namespace MqttLibNet.Packets.Handlers
{
    public class Subscribe : BasePacketHandler<SubscribeData>
    {
        public Subscribe()
               : base(MqttControlPacketType.Subscribe)
        {
        }

        protected override byte[] GetVariableHeaders(SubscribeData data)
        {
            return data.PacketIdentifier.GetMqttInt16Bytes();
        }

        protected override byte[] GetPayload(SubscribeData data)
        {
            List<byte> payload = new List<byte>();
            foreach (var subscription in data.Subscriptions)
            {
                payload.AddRange(subscription.TopicName.GetMqttUTF8EncodedString());
                payload.Add((byte)subscription.Qos);
            }

            return payload.ToArray();
        }
    }
}
