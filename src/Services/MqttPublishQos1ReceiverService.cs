using System;
using MqttLibNet.IO;
using MqttLibNet.Packets;
using MqttLibNet.Packets.Data;
using MqttLibNet.Packets.Handlers;
using MqttLibNet.Utils;

namespace MqttLibNet.Services
{
    public class MqttPublishQos1ReceiverService : IObserver<(byte[] Data, byte Flag)>
    {
        private readonly MqttStreamReaderWriter mqttStreamReaderWriter;

        public MqttPublishQos1ReceiverService(MqttStreamReaderWriter mqttStreamReaderWriter)
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

        public async void OnNext((byte[] Data, byte Flag) qos1)
        {
            PublishQos1 publish = new PublishQos1();
            var publishQos1 = publish.Deserialize(qos1.Data);
            PubAck pubAck = new PubAck();
            PubAckData pubAckData = new PubAckData();
            pubAckData.PacketIdentifier = publishQos1.PacketIdentifier;
            var pubAckBytes = pubAck.Serialize(pubAckData);
            await mqttStreamReaderWriter.WriteAsync(pubAckBytes);
            Console.WriteLine($"Topic -{publishQos1.TopicName} Qos -{QosLevel.Qos1} Message -{publishQos1.Message}");
        }

        public void Start()
        {
            mqttStreamReaderWriter.Subscribe(
                this, 
                _ => _.packetType == MqttControlPacketType.Publish &&
                 _.flag.IsBitOn(1) &&
                 !_.flag.IsBitOn(2));
        }
    }
}