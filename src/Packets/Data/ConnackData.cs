namespace MqttLibNet.Packets.Data
{
    public class ConnackData
    {
        public bool SessionPresent { get; set; }
        public ConnectReturnCode ConnectReturnCode { get; set; }
    }
}
