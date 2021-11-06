using MqttLibNet.Packets;
using System;

namespace MqttLibNet.Utils
{
    public static class MqttControlPacketTypeExtension
    {
        /// <summary>
        /// Publish and Subscribe have these flags in 0-3 bits
        /// </summary>
        /// <param name="mqttControlPacketType"></param>
        /// <returns></returns>
        public static byte GetFlag(this MqttControlPacketType mqttControlPacketType)
        {
            byte flag = 0b_00000000;
            if(mqttControlPacketType == MqttControlPacketType.Subscribe)
            {
                flag = 0b_00000010;
            }
            // TODO may be later move this logic here 
            // for now it is embedded in Publish.cs handler 
            // as it also require the knowledge of Retai/DUP/Qos levels.
            if(mqttControlPacketType == MqttControlPacketType.Publish)
            {
                throw new NotImplementedException();
            }
            return flag;
        }
    }
}
