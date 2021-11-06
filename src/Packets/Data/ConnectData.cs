namespace MqttLibNet.Packets.Data
{
    public class ConnectData
    {
        public string ProtocolName { get; set; }
        public short ProtocolLevel { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public short KeepAliveMs { get; set; }
        public string ClientIdentifier { get; set; }
        public string WillTopic { get; set; }
        public string WillMessage { get; set; }
        public QosLevel WillQos { get; set; }
        public bool WillRetain { get; set; }
        public bool CleanSession { get; set; }
    }
}
