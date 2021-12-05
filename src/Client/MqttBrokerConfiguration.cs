namespace MqttLibNet.Client
{
    public class MqttBrokerConfiguration
    {
        public string Broker { get; set; }
        public int Port { get; set; }
        public bool SSLEnabled { get; set; } = false;
        public int ReadWriteTimeoutMs { get; set; } = 60000;
    }
}
