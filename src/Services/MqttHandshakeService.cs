using MqttLibNet.IO;
using MqttLibNet.Packets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MqttLibNet.Services
{
    public class MqttHandshakeService : IObserver<byte[]>
    {
        private readonly MqttStreamReaderWriter mqttStreamReaderWriter;
        private readonly SemaphoreSlim semaphoreSlim;
        private ConnackData connackData;
        public MqttHandshakeService(
            MqttStreamReaderWriter mqttStreamReaderWriter)
        {
            this.mqttStreamReaderWriter = mqttStreamReaderWriter;
            this.semaphoreSlim = new SemaphoreSlim(0, 1);
        }

        public async Task<ConnackData> StartAsync(ConnectData connectData)
        {
            Connect connect = new Connect();
            var connectPacket = connect.Serialize(connectData);
            mqttStreamReaderWriter.Subscribe(this, _ => _.packetType == MqttControlPacketType.Connack);
            await mqttStreamReaderWriter.WriteAsync(connectPacket);
            await semaphoreSlim.WaitAsync();
            return connackData;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(byte[] connackPacket)
        {
            Connack connack = new Connack();
            connackData = connack.Deserialize(connackPacket);
            semaphoreSlim.Release();
        }
    }
}
