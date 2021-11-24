using MqttLibNet.Client;

namespace MqttLibNet.Packets.Data
{
    public class ConnectData
    {
        public ConnectData(MqttClientConfiguration clientConfiguration)
        {
            ProtocolName = clientConfiguration.ProtocolName;
            ProtocolLevel = clientConfiguration.ProtocolLevel;
            UserName = clientConfiguration.UserName;
            Password = clientConfiguration.Password;
            KeepAliveMs = clientConfiguration.KeepAliveMs;
            ClientIdentifier = clientConfiguration.ClientIdentifier;
            WillTopic = clientConfiguration.WillTopic;
            WillMessage = clientConfiguration.WillMessage;
            WillQos = (QosLevel)clientConfiguration.WillQos;
            WillRetain = clientConfiguration.WillRetain;
            CleanSession = clientConfiguration.CleanSession;
        }
        public string ProtocolName { get; private set; }
        public short ProtocolLevel { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public short KeepAliveMs { get; private set; }
        public string ClientIdentifier { get; private set; }
        public string WillTopic { get; private set; }
        public string WillMessage { get; private set; }
        public QosLevel WillQos { get; private set; }
        public bool WillRetain { get; private set; }
        public bool CleanSession { get; private set; }
    }
}
