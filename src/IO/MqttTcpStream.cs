using System;
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

        // Tcp stream.
        private Lazy<TcpClient> tcpClientInternal;
        // SSL stream.
        private Lazy<SslStream> sslStream;

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
            sslStream = new Lazy<SslStream>(() =>
            {
                var ssl = new SslStream(tcpClientInternal.Value.GetStream(), false);
                ssl.AuthenticateAsClient(broker);
                return ssl;
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
            var bytesRead = sslEnabled ? await sslStream.Value.ReadAsync(buffer, 0, noOfBytes)
                                       : await tcpClientInternal.Value.GetStream().ReadAsync(buffer, 0, noOfBytes);
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
            if (sslEnabled) 
            {
#if  (NET5_0 || NETSTANDARD2_1)
                await sslStream.Value.WriteAsync(buffer);
#else
                await sslStream.Value.WriteAsync(buffer,0,buffer.Length);
#endif
            }
            else
            {
#if  (NET5_0 || NETSTANDARD2_1)
                await tcpClientInternal.Value.GetStream().WriteAsync(buffer);
#else
                await tcpClientInternal.Value.GetStream().WriteAsync(buffer,0,buffer.Length);
#endif
            }
            semaphoreSlimWriter.Release();
        }
    }
}
