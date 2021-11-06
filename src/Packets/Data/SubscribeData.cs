﻿using System.Collections.Generic;

namespace MqttLibNet.Packets.Data
{
    public class SubscribeData
    {
        public short PacketIdentifier { get; set; }
        public IEnumerable<(string TopicName, QosLevel Qos)> Subscriptions { get; set; }
    }
}