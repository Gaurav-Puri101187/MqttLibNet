using MqttLibNet.IO;
using MqttLibNet.Packets;
using MqttLibNet.Utils;
using System;
using System.Threading.Tasks;

namespace MqttLibNet.Services
{
    public class MqttPublishQos0Service : IObserver<byte[]>
    {
        private readonly MqttStreamReaderWriter mqttStreamReaderWriter;

        public MqttPublishQos0Service(MqttStreamReaderWriter mqttStreamReaderWriter)
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

        public void OnNext(byte[] value)
        {
            PublishQos0 publishQos0 = new PublishQos0();
            var data = publishQos0.Deserialize(value);
            Console.WriteLine($"Topic -{data.TopicName} Qos -{data.QosLevel} Message -{data.Message}");
        }

        public void Start()
        {
            this.mqttStreamReaderWriter.Subscribe(this, _ => 
            _.packetType == MqttControlPacketType.Publish);
        }

        public async Task PushAsync(PublishData publishData)
        {
            Publish publish = new PublishQos0();
            var pubData = publish.Serialize(publishData);
            await mqttStreamReaderWriter.WriteAsync(pubData);
        }
    }
}
