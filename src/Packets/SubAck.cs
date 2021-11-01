using MqttLibNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MqttLibNet.Packets
{
    public class SubAck : IMqttBaseControlPacket<SubAckData>
    {
        public MqttControlPacketType ControlPacketType => MqttControlPacketType.SubAck;

        /// <summary>
        /// This should be implemented on client side
        /// where client should receive and  deserialize the packet.
        /// </summary>
        /// <param name="packetBytes"></param>
        /// <returns></returns>
        public SubAckData Deserialize(byte[] packetBytes)
        {
            SubAckData subAckData = new SubAckData();
            subAckData.PacketIdentifier = packetBytes.Take(2).ToArray().GetMqttInt16();
            List<SubAckReturnCode> returnCodes = new List<SubAckReturnCode>();
            for(int i = 2; i <= packetBytes.Length - 1; ++i)
            {
                returnCodes.Add((SubAckReturnCode)packetBytes[i]);
            }
            subAckData.SubAckReturnCode = returnCodes;
            return subAckData;
        }

        /// <summary>
        /// This should be implemented on server side 
        /// where server would create the sub ack packet
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Serialize(SubAckData data)
        {
            throw new NotImplementedException();
        }
    }
}
