using System;

namespace MqttLibNet.Packets
{
    public class PingResp : IMqttBaseControlPacket<EmptyPacketData>
    {
        public MqttControlPacketType ControlPacketType => MqttControlPacketType.PingResp;

        public EmptyPacketData Deserialize(byte[] packetBytes)
        {
            return null;
        }

        public byte[] Serialize(EmptyPacketData data)
        {
            throw new NotImplementedException();
        }
    }
}
