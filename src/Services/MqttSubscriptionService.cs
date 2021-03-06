using MqttLibNet.IO;
using MqttLibNet.Packets;
using MqttLibNet.Packets.Data;
using MqttLibNet.Packets.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MqttLibNet.Services
{
    public class MqttSubscriptionService : IObserver<(byte[] Data, byte Flag)>
    {
        private readonly MqttStreamReaderWriter mqttStreamReaderWriter;
        private readonly SemaphoreSlim subscribeLock;
        private SubAckData subAckData;
        public MqttSubscriptionService(MqttStreamReaderWriter mqttStreamReaderWriter)
        {
            this.mqttStreamReaderWriter = mqttStreamReaderWriter;
            this.subscribeLock = new SemaphoreSlim(0, 1);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext((byte[] Data, byte Flag) subAckPacket)
        {
            SubAck subAck = new SubAck();
            subAckData = subAck.Deserialize(subAckPacket.Data);
            subscribeLock.Release();
        }

        public async Task<SubAckData> StartAsync(SubscribeData subscribeData)
        {
            Subscribe subscribe = new Subscribe();
            var subscribePacket = subscribe.Serialize(subscribeData);
            mqttStreamReaderWriter.Subscribe(this, _ => _.packetType == MqttControlPacketType.SubAck);
            await mqttStreamReaderWriter.WriteAsync(subscribePacket);
            await subscribeLock.WaitAsync();
            return subAckData;
        }

    }
}
