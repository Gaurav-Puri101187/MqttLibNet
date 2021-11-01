using System.Collections.Generic;
using System.Threading.Tasks;

namespace MqttLibNet.IO
{
    public interface IMqttStream
    {
        Task WriteAsync(byte[] buffer);
        Task<byte[]> ReadAsync(int noOfBytes);
        Task<byte> ReadByteAsync();
        void Reset();
    }
}
