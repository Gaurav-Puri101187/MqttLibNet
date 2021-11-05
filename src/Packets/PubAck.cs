using System.Collections.Generic;
using MqttLibNet.Utils;

namespace MqttLibNet.Packets
{
    public class PubAck : IMqttBaseControlPacket<PubAckData>
    {
        public MqttControlPacketType ControlPacketType => MqttControlPacketType.PubAck;

        public PubAckData Deserialize(byte[] packetBytes)
        {
            var packetIdentifier = (short)packetBytes.GetMqttInt16();
            PubAckData pubAck = new PubAckData();
            pubAck.PacketIdentifier = packetIdentifier;
            return pubAck;
        }

        public byte[] Serialize(PubAckData data)
        {
            List<byte> pubAckBytes = new List<byte>();
            // step 1 Add the ControlType Packet header.
            pubAckBytes.Add((byte)ControlPacketType);
            // step 2 create variable headers
            List<byte> variableHeader = new List<byte>();
            variableHeader.AddRange(data.PacketIdentifier.GetMqttBigEndianInt16());
            // step 3 add variable header length to the packet.
            pubAckBytes.AddRange(variableHeader.Count.GetMqttRemainingLength());
            // step 4 add variable headers.
            pubAckBytes.AddRange(variableHeader);
            return pubAckBytes.ToArray();
        }
    }
}