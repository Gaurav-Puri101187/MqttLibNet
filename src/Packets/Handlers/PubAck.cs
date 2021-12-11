using MqttLibNet.Packets.Data;
using MqttLibNet.Utils;

namespace MqttLibNet.Packets.Handlers
{
    public class PubAck : BasePacketHandler<PubAckData>
    {
        public PubAck()
               : base(MqttControlPacketType.PubAck)
        {
        }

        /// <summary>
        /// This needs to be implemented on both server and client side
        /// as both can receive the PubAck.
        /// </summary>
        /// <param name="packetBytes"></param>
        /// <returns></returns>
        public override PubAckData Deserialize(byte[] packetBytes)
        {
            var packetIdentifier = (short)packetBytes.GetMqttInt16();
            PubAckData pubAck = new PubAckData();
            pubAck.PacketIdentifier = packetIdentifier;
            return pubAck;
        }

        protected override byte[] GetVariableHeaders(PubAckData data)
        {
            return data.PacketIdentifier.GetMqttInt16Bytes();
        }
    }
}