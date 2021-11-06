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
            if(value == 0)
            {
                remainingLengthByte.Add((byte)(value));
            }
            while(value % 128 != 0)
            {
                remainingLengthByte.Add((byte)(value % 128));
                value = value - (value % 128);
            }
            if(value > 0)
            {
                remainingLengthByte.Add((byte)(value / 128));
            }
            return remainingLengthByte.ToArray();
        }
    }
}
