using System.Collections.Generic;

namespace MqttLibNet.Packets
{
    public class PingReq : IMqttBaseControlPacket<EmptyPacketData>
    {
        public MqttControlPacketType ControlPacketType => MqttControlPacketType.PingReq;

        public EmptyPacketData Deserialize(byte[] packetBytes)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Serialize(EmptyPacketData data)
        {
            List<byte> pingReqPacketBytes = new List<byte>();

            // Step 1 create packet type header byte.
            pingReqPacketBytes.Add((byte)ControlPacketType);
            // 0 as Remaining length
            pingReqPacketBytes.Add(0);
            return pingReqPacketBytes.ToArray();
        }
    }
}
