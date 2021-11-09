using MqttLibNet.IO;
using MqttLibNet.Packets;
using MqttLibNet.Packets.Data;
using MqttLibNet.Packets.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MqttLibNet.Services
{
    /// <summary>
    /// This workflow uses a DistinctUntilChanged kind of mechanism 
    /// prevIdentifierReceived will be the identifier last received acknowledgement for
    /// there may be a chance that some retry has happened and so some dup packets been sent to broker
    /// but we will need only one ack to start pushing new message
    /// runningIdentifier is what helps to distinguish between the packets.
    /// </summary>
    public class MqttPublishQos1DispatchService : IObserver<(byte[] Data, byte Flag)>
    {
        private readonly MqttStreamReaderWriter mqttStreamReaderWriter;
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        private SemaphoreSlim _lock = new SemaphoreSlim(1,1);
        private short runningIdentifier;
        private short prevIdentifierReceived;
        private int counter;
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
            if(prevIdentifierReceived != data.PacketIdentifier &&
                data.PacketIdentifier == runningIdentifier)
            {
                Console.WriteLine($"Ack for packet {data.PacketIdentifier}");
                prevIdentifierReceived = runningIdentifier;
                semaphoreSlim.Release();
            }
        }

        public async Task PublishAsync(PublishData publishData)
        {
            await _lock.WaitAsync();
            runningIdentifier = (short)(Interlocked.Increment(ref counter) % 65536);
            publishData.PacketIdentifier = runningIdentifier;
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
