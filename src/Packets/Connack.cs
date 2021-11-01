using System;

namespace MqttLibNet.Packets
{
    public class Connack : IMqttBaseControlPacket<ConnackData>
    {
        public MqttControlPacketType ControlPacketType => MqttControlPacketType.Connack;

        /// <summary>
        /// A client would always deserialize a connack
        /// </summary>
        /// <param name="packetBytes"></param>
        /// <returns></returns>
        public ConnackData Deserialize(byte[] packetBytes)
        {
            ConnackData connackData = new ConnackData();
            // first byte would tell us wether we have session flag on or not.
            connackData.SessionPresent = packetBytes[0] == 1;
            // second byte would tell us the return code.
            connackData.ConnectReturnCode = (ConnectReturnCode)packetBytes[1];
            return connackData;
        }

        /// <summary>
        /// A server will always prepare/serialize the connack message.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Serialize(ConnackData data)
        {
            throw new NotImplementedException();
        }
    }
}
