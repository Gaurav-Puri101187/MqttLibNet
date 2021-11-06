namespace MqttLibNet.Packets.Data
{
    public class PublishData
    {
        public bool Duplicate { get; set; }
        public QosLevel QosLevel { get; set; }
        public bool Retain { get; set; }
        public string TopicName { get; set; }
        public short PacketIdentifier { get; set; }
        public string Message { get; set; }
    }
}
