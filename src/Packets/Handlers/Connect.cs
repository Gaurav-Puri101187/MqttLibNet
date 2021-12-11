using MqttLibNet.Packets.Data;
using MqttLibNet.Utils;
using System.Collections.Generic;

namespace MqttLibNet.Packets.Handlers
{
    public class Connect : BasePacketHandler<ConnectData>
    {

        public Connect()
               : base(MqttControlPacketType.Connect)
        {
        }

        protected override byte[] GetPayload(ConnectData connectData)
        {
            List<byte> payload = new List<byte>();
            payload.AddRange(connectData.ClientIdentifier.GetMqttUTF8Bytes());
            if (!string.IsNullOrEmpty(connectData.WillTopic))
            {
                payload.AddRange(connectData.WillTopic.GetMqttUTF8Bytes());
            }
            if (!string.IsNullOrEmpty(connectData.WillMessage))
            {
                payload.AddRange(connectData.WillMessage.GetMqttUTF8Bytes());
            }
            if (!string.IsNullOrEmpty(connectData.UserName))
            {
                payload.AddRange(connectData.UserName.GetMqttUTF8Bytes());
            }
            if (!string.IsNullOrEmpty(connectData.Password))
            {
                payload.AddRange(connectData.Password.GetMqttUTF8Bytes());
            }
            return payload.ToArray();
        }

        protected override byte[] GetVariableHeaders(ConnectData connectData)
        {
            List<byte> variableHeaders = new List<byte>();
            variableHeaders.AddRange(connectData.ProtocolName.GetMqttUTF8Bytes());
            variableHeaders.Add((byte)connectData.ProtocolLevel);
            variableHeaders.Add(GetFlag(connectData));
            variableHeaders.AddRange(((short)(((connectData.KeepAliveMs) / 1000))).GetMqttInt16Bytes());
            return variableHeaders.ToArray();
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
