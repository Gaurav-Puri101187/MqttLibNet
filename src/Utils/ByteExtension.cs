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
            var operand = (byte)Math.Pow(2, bitPosition);
            return (b & operand) == operand;
        }

        /// <summary>
        /// Mqtt numbers are stored as MSB and then LSB 
        /// This method expects to receive them in similar format
        /// we need to reverse and bytes before converting them to a number.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int GetMqttInt16(this byte[] bytes)
        {
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes);
        }
    }
}
