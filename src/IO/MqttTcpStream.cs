using System;
using System.IO;
using System.Net.Security;
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
        private readonly string broker;
        private readonly int port;
        private readonly bool sslEnabled;
        private readonly int readWriteTimeoutMs;

        // Tcp client raw
        private Lazy<TcpClient> tcpClientInternal;
        // Network/SSL stream
        private Lazy<Stream> streamInternal;

        public MqttTcpStream(string broker, int port, bool sslEnabled, int readWriteTimeoutMs)
        {
            semaphoreSlimWriter = new SemaphoreSlim(1, 1);
            semaphoreSlimReader = new SemaphoreSlim(1, 1);
            this.broker = broker;
            this.port = port;
            this.sslEnabled = sslEnabled;
            this.readWriteTimeoutMs = readWriteTimeoutMs;
            InitClient();
        }

        private void InitClient()
        {
            tcpClientInternal = new Lazy<TcpClient>(() =>
            {
                return new TcpClient(broker, port) { ReceiveTimeout = readWriteTimeoutMs, SendTimeout = readWriteTimeoutMs };
            }, true);
            streamInternal = new Lazy<Stream>(() =>
            {
                if (sslEnabled)
                {
                    var ssl = new SslStream(tcpClientInternal.Value.GetStream(), false);
                    ssl.AuthenticateAsClient(broker);
                    return ssl;
                }
                else { return tcpClientInternal.Value.GetStream(); }
            }, true);
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
            var bytesRead = await streamInternal.Value.ReadAsync(buffer, 0, noOfBytes);
            semaphoreSlimReader.Release();
            if (bytesRead == 0) { throw new Exception("Read Timed Out!!!"); }
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
            if (streamInternal.IsValueCreated)
            {
                streamInternal.Value.Dispose();
                tcpClientInternal.Value.Close();
                tcpClientInternal.Value.Dispose();
            }
            InitClient();
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
#if (NET5_0 || NETSTANDARD2_1)
            await streamInternal.Value.WriteAsync(buffer);
#else
            await streamInternal.Value.WriteAsync(buffer,0,buffer.Length);
#endif
            semaphoreSlimWriter.Release();
        }
    }
}
