using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MqttLibNet.IO
{
    public class MqttTcpStream : IMqttStream
    {
        // Slim lock for Writer.
        private readonly SemaphoreSlim semaphoreSlimWriter;
        // Slim lock for Reader.
        private readonly SemaphoreSlim semaphoreSlimReader;
        // Tcp stream.
        private Lazy<TcpClient> tcpClientInternal;
        // Timeout for readandwrite.
        private const int ReadWriteTimeout = 60000;
        public MqttTcpStream()
        {
            semaphoreSlimWriter = new SemaphoreSlim(1, 1);
            semaphoreSlimReader = new SemaphoreSlim(1, 1);
            tcpClientInternal = new Lazy<TcpClient>(() =>
            {
                return new TcpClient("broker.hivemq.com", 1883) { ReceiveTimeout = ReadWriteTimeout, SendTimeout = ReadWriteTimeout };
            });
        }

        /// <summary>
        /// A simple read method for the underlying stream
        /// It will read  it in async fashion on first come first serve basis
        /// It is thread safe in behavior, will allow only one read at a time.
        /// </summary>
        /// <param name="noOfBytes"></param>
        /// <returns></returns>
        public async Task<byte[]> ReadAsync(int noOfBytes)
        {
            await semaphoreSlimReader.WaitAsync();
            byte[] buffer = new byte[noOfBytes];
            var bytesRead = await tcpClientInternal.Value.GetStream().ReadAsync(buffer, 0, noOfBytes);
            if(bytesRead == 0)
            {
                throw new Exception("Read Timed Out!!!");
            }
            semaphoreSlimReader.Release();
            return buffer;
        }

        public async Task<byte> ReadByteAsync()
        {
            var item = await ReadAsync(1);
            byte byteToReturn = item[0];
            return byteToReturn;
        }

        /// <summary>
        /// Whenever this method is called it will close and dispose the existing TCP connection
        /// And will initialize a new one.
        /// </summary>
        public void Reset()
        {
            tcpClientInternal.Value.Close();
            tcpClientInternal.Value.Dispose();
            tcpClientInternal = new Lazy<TcpClient>(() =>
            {
                return new TcpClient("broker.hivemq.com", 1883);
            });
        }

        /// <summary>
        /// A simple write method for the underlying stream
        /// It will write to it in async fashion on first come first serve basis
        /// It is thread safe in behavior as will allow only one write at a time.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public async Task WriteAsync(byte[] buffer)
        {
            await semaphoreSlimWriter.WaitAsync();
            await tcpClientInternal.Value.GetStream().WriteAsync(buffer);
            semaphoreSlimWriter.Release();
        }
    }
}
