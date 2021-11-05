using MqttLibNet.Utils;
using System.Collections.Generic;

namespace MqttLibNet.Packets
{
    public class Connect : IMqttBaseControlPacket<ConnectData>
    {
        public MqttControlPacketType ControlPacketType => MqttControlPacketType.Connect;

        /// <summary>
        /// This is supposed to be implemented on server side
        /// As client would bever receive the Connect packet.
        /// </summary>
        /// <param name="packetBytes"></param>
        /// <returns></returns>
        public ConnectData Deserialize(byte[] packetBytes)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// This is supposed to be implemented on client side
        /// As server would never send a connect packet.
        /// </summary>
        /// <param name="connectData"></param>
        /// <returns></returns>
        public byte[] Serialize(ConnectData connectData)
        {
            List<byte> connectPacketBytes = new List<byte>();

            // Step 1 create packet type header byte.
            connectPacketBytes.Add((byte)ControlPacketType);

            // Step 2 create variable headers
            List<byte> variableHeaders = new List<byte>();
            variableHeaders.AddRange(connectData.ProtocolName.GetMqttUTF8EncodedString());
            variableHeaders.Add((byte)connectData.ProtocolLevel);
            variableHeaders.Add(GetFlag(connectData));
            variableHeaders.AddRange(((short)(((connectData.KeepAliveMs)/1000))).GetMqttBigEndianInt16());

            // Step 3 create payload
            List<byte> payload = new List<byte>();
            payload.AddRange(connectData.ClientIdentifier.GetMqttUTF8EncodedString());
            if (!string.IsNullOrEmpty(connectData.WillTopic))
            {
                payload.AddRange(connectData.WillTopic.GetMqttUTF8EncodedString());
            }
            if (!string.IsNullOrEmpty(connectData.WillMessage))
            {
                payload.AddRange(connectData.WillMessage.GetMqttUTF8EncodedString());
            }
            if (!string.IsNullOrEmpty(connectData.UserName))
            {
                payload.AddRange(connectData.UserName.GetMqttUTF8EncodedString());
            }
            if (!string.IsNullOrEmpty(connectData.Password))
            {
                payload.AddRange(connectData.Password.GetMqttUTF8EncodedString());
            }

            // Step 4 Make a variable Add the variable headers and payload in the packet.
            var headerLength = variableHeaders.Count + payload.Count;
            connectPacketBytes.AddRange(headerLength.GetMqttRemainingLength());
            connectPacketBytes.AddRange(variableHeaders);
            connectPacketBytes.AddRange(payload);
            return connectPacketBytes.ToArray();
        }

        private byte GetFlag(ConnectData connectData)
        {
            byte flag = 0b_00000000;
            // connect flag byte is part of connect variable headers
            // Byte structure is below its like MSb first
            // UNameFlag(1 bit),
            // PwdFlag(1 bit),
            // WillRetainFlag(1 bit),
            // WillQosFlag(2 bits),
            // WillFlag(1 bit),
            // CleanSession(1 bit), 
            // Reserved(1 bit) and is always 0;

            if (!string.IsNullOrEmpty(connectData.UserName) && 
                !string.IsNullOrEmpty(connectData.Password))
            {
                flag = (byte)(flag | 0b_11000000);
            }

            if (connectData.WillRetain)
            {
                flag = (byte)(flag | 0b_00100000);
            }

            // We need to do shift left by 3 before bit or 
            // because Qos is (00000000, 00000001, 00000010) and its flag position is 3 and 4.
            flag = (byte)(flag | (byte)((byte)connectData.WillQos << 3));

            if(!string.IsNullOrEmpty(connectData.WillTopic) &&
                !string.IsNullOrEmpty(connectData.WillMessage))
            {
                flag = (byte)(flag | 0b_00000100);
            }

            if (connectData.CleanSession)
            {
                flag = (byte)(flag | 0b_00000010);
            }
            return flag;
        }
    }
}
