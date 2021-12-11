using System;

namespace MqttLibNet.Utils
{
    public static class ByteExtension
    {
        /// <summary>
        /// To check wether a bit position is on in a byte Bit Position starts from index 0.
        /// Just bit and it with the number whose only bit at that position is ON
        /// For ex. 11000000 to check wethere bit position 7 is ON
        /// just do 11000000 & 01000000 if the result is 01000000 then BIT os ON else OFF.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="bitPosition"></param>
        /// <returns></returns>
        public static bool IsBitOn(this byte b, int bitPosition)
        {
            return (b & (1 << bitPosition)) != 0;
        }

        /// <summary>
        /// Mqtt numbers are stored as MSB and then LSB 
        /// This method expects to receive them in similar format
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int GetMqttInt16(this byte[] bytes, int start = 0)
        {
            return (((int)bytes[start]) << 8) | ((int)bytes[start + 1]);
        }

        /// <summary>
        /// Mqtt strings are stored in UTF8 encoded pascal string. First
        /// 2 bytes contains the length in an int16_be.
        /// This method expects to receive them in similar format
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetMqttUTF8String(this byte[] bytes, int start = 0)
        {
            int utf8ByteCount = GetMqttInt16(bytes, start);

            return System.Text.Encoding.UTF8.GetString(bytes, start + 2, utf8ByteCount);
        }
    }
}
