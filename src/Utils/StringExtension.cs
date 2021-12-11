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
        public static byte[] GetMqttUTF8Bytes(this string str)
        {
            var utf8Bytes = Encoding.UTF8.GetBytes(str);
            var lenInt16Bytes = ((short)utf8Bytes.Length).GetMqttInt16Bytes();
            List<byte> byteString = new List<byte>(utf8Bytes.Length + lenInt16Bytes.Length);
            byteString.AddRange(lenInt16Bytes);
            byteString.AddRange(utf8Bytes);
            return byteString.ToArray();
        }
    }
}
