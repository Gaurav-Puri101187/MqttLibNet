using MqttLibNet.Packets.Data;
using System.Collections.Generic;

namespace MqttLibNet.Packets.Handlers
{
    public class PingReq : BasePacketHandler<EmptyPacketData>
    {

        public PingReq()
               : base(MqttControlPacketType.PingReq)
        {
        }
    }
}
