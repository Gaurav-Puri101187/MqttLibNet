using System.Collections.Generic;

namespace MqttLibNet.Packets.Data
{
    public class SubAckData
    {
        public int PacketIdentifier { get; set; }
        public IEnumerable<SubAckReturnCode> SubAckReturnCode { get; set; }
    }
}
