using MqttLibNet.Packets.Data;
using MqttLibNet.Utils;
using System.Collections.Generic;

namespace MqttLibNet.Packets.Handlers
{
    public abstract class Publish : IMqttBaseControlPacket<PublishData>
    {
        public MqttControlPacketType ControlPacketType => MqttControlPacketType.Publish;

        /// <summary>
        /// This needs to be implemented by both server and client
        /// as both can send and receive messages.
        /// </summary>
        /// <param name="packetBytes"></param>
        /// <returns></returns>
        public abstract PublishData Deserialize(byte[] packetBytes);

        /// <summary>
        /// This needs to be implemented by both server and client
        /// as both can send and receive messages.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual byte[] Serialize(PublishData publishData)
        {
            List<byte> publishPacketBytes = new List<byte>();

            // Step 1 create packet type header byte.
            var header = (byte)ControlPacketType;
            if(publishData.QosLevel > QosLevel.Qos0)
            {
                // first 4 bits i.e. 7-4 of header are control packet type
                // 3rd one shows wether it is duplicate message or not.
                if (publishData.Duplicate)
                {
                    header = (byte)(header | 0b_00001000);
                }
                // set the 2 and 1 bit basis on Qos level.
                header = (byte)(header | (byte)publishData.QosLevel << 1);
                // set the 0th bit based upon retain flag.
                if (publishData.Retain)
                {
                    header = (byte)(header | 0b_00000001);
                }
            }
            publishPacketBytes.Add(header);

            // step 2 prepare variable headers
            List<byte> variableHeaders = new List<byte>();
            variableHeaders.AddRange(publishData.TopicName.GetMqttUTF8Bytes());
            if(publishData.QosLevel > QosLevel.Qos0)
            {
                variableHeaders.AddRange(publishData.PacketIdentifier.GetMqttInt16Bytes());
            }

            // step 3 prepare payload.
            var payload = publishData.Message.GetMqttUTF8Bytes();

            // step 4 add remaining length then add variable headers and then payload.
            int remainingLength = variableHeaders.Count + payload.Length;
            publishPacketBytes.AddRange(remainingLength.GetMqttRemainingLength());
            publishPacketBytes.AddRange(variableHeaders);
            publishPacketBytes.AddRange(payload);

            return publishPacketBytes.ToArray();
        }
    }
}
