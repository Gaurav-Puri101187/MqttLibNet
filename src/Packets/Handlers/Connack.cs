using MqttLibNet.Packets.Data;
using System;

namespace MqttLibNet.Packets.Handlers
{
    public class Connack : BasePacketHandler<ConnackData>
    {
        public Connack()
               : base(MqttControlPacketType.Connack)
        {
        }
        /// <summary>
        /// A client would always deserialize a connack
        /// </summary>
        /// <param name="packetBytes"></param>
        /// <returns></returns>
        public override ConnackData Deserialize(byte[] packetBytes)
        {
            ConnackData connackData = new ConnackData();
            // first byte would tell us wether we have session flag on or not.
            connackData.SessionPresent = packetBytes[0] == 1;
            // second byte would tell us the return code.
            connackData.ConnectReturnCode = (ConnectReturnCode)packetBytes[1];
            return connackData;
        }
    }
}
