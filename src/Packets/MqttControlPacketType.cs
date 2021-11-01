namespace MqttLibNet.Packets
{
    /// <summary>
    /// These are the control packets defined by the MQTT specification
    /// </summary>
    public enum MqttControlPacketType
    {
        None=0b_00000000,
        Connect=0b_00010000,
        Connack=0b_00100000,
        Publish=0b_00110000,
        Subscribe=0b_10000000,
        SubAck=0b_10010000,
        PingReq=0b_11000000,
        PingResp=0b_11010000,
        Disconnect=0b_11110000
    }
}
