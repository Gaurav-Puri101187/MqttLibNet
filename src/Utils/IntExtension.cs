using System;
using System.Collections.Generic;

namespace MqttLibNet.Utils
{
    public static class IntExtension
    {
        /// <summary>
        /// MQTT supports BigEndian int (MSB first, then LSB)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetMqttInt16Bytes(this short value)
        {
            var bytes = new byte[2];

            bytes[0] = (byte)(value >> 8);
            bytes[1] = (byte)value;

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
