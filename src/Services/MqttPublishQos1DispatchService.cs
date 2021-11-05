using MqttLibNet.IO;
using MqttLibNet.Packets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MqttLibNet.Services
{
    public class MqttPublishQos1DispatchService : IObserver<(byte[] Data, byte Flag)>
    {
        private readonly MqttStreamReaderWriter mqttStreamReaderWriter;
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public MqttPublishQos1DispatchService(MqttStreamReaderWriter mqttStreamReaderWriter)
        {
            this.mqttStreamReaderWriter = mqttStreamReaderWriter;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext((byte[] Data, byte Flag) pubAckBytes)
        {
            PubAck pubAck = new PubAck();
            var data = pubAck.Deserialize(pubAckBytes.Data);
            if(data.PacketIdentifier == 1)
            {
                semaphoreSlim.Release();
            }
            Console.WriteLine($"Ack for packet {data.PacketIdentifier}");
        }

        public async Task PushAsync(PublishData publishData)
        {
            await semaphoreSlim.WaitAsync();
            PublishQos1 publishQos1 = new PublishQos1();
            publishData.QosLevel = QosLevel.Qos1;
            publishData.PacketIdentifier = 1;
            var qos1Bytes = publishQos1.Serialize(publishData);
            await mqttStreamReaderWriter.WriteAsync(qos1Bytes);
        }

        public void Start()
        {
            mqttStreamReaderWriter.Subscribe(
                this,
                _ => _.packetType == MqttControlPacketType.PubAck);
        }
    }
}
