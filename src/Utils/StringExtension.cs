using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLibNet.Utils
{
    public static class StringExtension
    {
        /// <summary>
        /// Get the length of the string to encode.
        /// If Arch is LittleEndian i.e. LSB MSB then reverese it as the MQTT supports BigEndian
        /// Just fill the array first with the length bytes.
        /// Then fill the rest of the string content.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetMqttUTF8EncodedString(this string str)
        {
            List<byte> byteString = new List<byte>(str.Length + 2);
            var lengthBytes = BitConverter.GetBytes((short)str.Length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthBytes);
            }
            byteString.AddRange(lengthBytes);
            byteString.AddRange(Encoding.UTF8.GetBytes(str));
            return byteString.ToArray();
        }
    }
}
