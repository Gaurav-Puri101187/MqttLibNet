namespace MqttLibNet.Packets
{
    public interface IMqttBaseControlPacket<TData>
    {
        MqttControlPacketType ControlPacketType { get; }
        byte[] Serialize(TData data);
        TData Deserialize(byte[] packetBytes);
    }
}
