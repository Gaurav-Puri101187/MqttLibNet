using MqttLibNet.Utils;
using System;
using System.Collections.Generic;

namespace MqttLibNet.Packets
{
    public class Subscribe : IMqttBaseControlPacket<SubscribeData>
    {
        public MqttControlPacketType ControlPacketType => MqttControlPacketType.Subscribe;

        /// <summary>
        /// This should be implemented on server side as server will be receiving
        /// this subscription request from client.
        /// </summary>
        /// <param name="packetBytes"></param>
        /// <returns></returns>
        public SubscribeData Deserialize(byte[] packetBytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This should be implemented on client side as client will be the one
        /// sending the subscription request to the server.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Serialize(SubscribeData subscribeData)
        {
            List<byte> subscribePacketBytes = new List<byte>();

            // Step 1 create packet type header byte.
            var header = (byte)ControlPacketType | 0b_00000010;
            subscribePacketBytes.Add((byte)header);

            // Step 2 create variable header bytes which will contain packet identifier
            // it will be a 2 byte long big endian number.
            byte[] variableHeaders;
            variableHeaders = subscribeData.PacketIdentifier.GetMqttBigEndianInt16();

            // Step 3 create payload 
            List<byte> payload = new List<byte>();
            foreach(var subscription in subscribeData.Subscriptions)
            {
                payload.AddRange(subscription.TopicName.GetMqttUTF8EncodedString());
                payload.Add((byte)subscription.Qos);
            }

            // step 4 calculate length and add the variable and then payload.
            var remainingLength = variableHeaders.Length + payload.Count;
            subscribePacketBytes.AddRange(remainingLength.GetMqttRemainingLength());
            subscribePacketBytes.AddRange(variableHeaders);
            subscribePacketBytes.AddRange(payload);

            return subscribePacketBytes.ToArray();
        }
    }
}
