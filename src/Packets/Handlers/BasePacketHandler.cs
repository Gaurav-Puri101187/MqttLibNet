using MqttLibNet.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLibNet.Packets.Handlers
{
    public abstract class BasePacketHandler<TData> : IMqttBaseControlPacket<TData>
    {
        public MqttControlPacketType ControlPacketType { get; private set; }

        public BasePacketHandler(MqttControlPacketType mqttControlPacketType)
        {
            ControlPacketType = mqttControlPacketType;
        }

        public virtual byte[] Serialize(TData data)
        {
            List<byte> packetBytes = new List<byte>();
            
            // Step 1 Add the control packet type header.
            packetBytes.Add((byte)((byte)ControlPacketType | ControlPacketType.GetFlag()));
            // Step 2 get variable headers.
            var variableHeaders = GetVariableHeaders(data);
            // Step 3 get payload.
            var payload = GetPayload(data);
            // step 4 Calculate remaining length and add variable headers and payload.
            var remainingLength = variableHeaders.Length + payload.Length;
            packetBytes.AddRange(remainingLength.GetMqttRemainingLength());
            packetBytes.AddRange(variableHeaders);
            packetBytes.AddRange(payload);
            return packetBytes.ToArray();
        }

        public virtual TData Deserialize(byte[] packetBytes)
        {
            throw new NotImplementedException();
        }

        protected virtual byte[] GetVariableHeaders(TData data)
        {
            return new byte[0];
        }

        protected virtual byte[] GetPayload(TData data)
        {
            return new byte[0];
        }
    }
}
