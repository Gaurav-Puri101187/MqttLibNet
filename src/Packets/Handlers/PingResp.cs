using MqttLibNet.Packets.Data;

namespace MqttLibNet.Packets.Handlers
{
    public class PingResp : BasePacketHandler<EmptyPacketData>
    {
        public PingResp()
               : base(MqttControlPacketType.PingResp)
        {
        }
    }
}
