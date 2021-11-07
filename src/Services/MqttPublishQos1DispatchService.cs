using MqttLibNet.IO;
using MqttLibNet.Packets;
using MqttLibNet.Packets.Data;
using MqttLibNet.Packets.Handlers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MqttLibNet.Services
{
    public class MqttPublishQos1DispatchService : IObserver<(byte[] Data, byte Flag)>
    {
        private readonly MqttStreamReaderWriter mqttStreamReaderWriter;
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _lock = new SemaphoreSlim(1,1);
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

        public async Task PublishAsync(PublishData publishData)
        {
            await _lock.WaitAsync();
            await PublishInternal(publishData);
            while (!await semaphoreSlim.WaitAsync(10000))
            {
                publishData.Duplicate = true;
                await PublishInternal(publishData);
            }
            _lock.Release();
        }

        private async Task PublishInternal(PublishData publishData)
        {
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
