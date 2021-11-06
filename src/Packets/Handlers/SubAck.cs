using MqttLibNet.Packets.Data;
using MqttLibNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MqttLibNet.Packets.Handlers
{
    public class SubAck : BasePacketHandler<SubAckData>
    {
        public SubAck()
               : base(MqttControlPacketType.SubAck)
        {
        }

        /// <summary>
        /// This should be implemented on client side
        /// where client should receive and  deserialize the packet.
        /// </summary>
        /// <param name="packetBytes"></param>
        /// <returns></returns>
        public override SubAckData Deserialize(byte[] packetBytes)
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
    }
}
