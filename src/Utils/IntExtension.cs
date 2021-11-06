using System;
using System.Collections.Generic;

namespace MqttLibNet.Utils
{
    public static class IntExtension
    {
        /// <summary>
        /// MQTT supports BigEndian int so reverse in case architecture is LittleEndian.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetMqttBigEndianInt16(this short value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        public static byte[] GetMqttRemainingLength(this int value)
        {
            List<byte> remainingLengthByte = new List<byte>();
            do
            {
                var rem = value % 128;
                if(value >= 128)
                {
                    rem = rem + 128;
                }
                remainingLengthByte.Add((byte)rem);
                value = value / 128;
            } while (value > 0);
            return remainingLengthByte.ToArray();
        }
    }
}
