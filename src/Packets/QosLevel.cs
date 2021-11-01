namespace MqttLibNet.Packets
{
    public enum QosLevel
    {
        /// <summary>
        /// Qos0 means at most once delivery semantic with no guaranty of delivery.
        /// </summary>
        Qos0 = 0b_00000000,
        /// <summary>
        /// Qos1 means at least once delivery semantic with guaranty that message will be delivered once.
        /// But could be delivered more than once so consumers should be idempotent.
        /// </summary>
        Qos1 = 0b_00000001,
        /// <summary>
        /// Qos2 means exactally once delivery semantics
        /// </summary>
        Qos2 = 0b_00000010
    }
}
