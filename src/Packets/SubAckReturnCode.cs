namespace MqttLibNet.Packets
{
    public enum SubAckReturnCode
    {
        /// <summary>
        /// Success - Maximum QoS 0 
        /// </summary>
        SuccessWithQos0 = 0x00,
        /// <summary>
        /// Success - Maximum QoS 1 
        /// </summary>
        SuccessWithQos1 = 0x01,
        /// <summary>
        /// Success - Maximum QoS 2 
        /// </summary>
        SuccessWithQos2 = 0x02,
        /// <summary>
        /// Failure
        /// </summary>
        Failure=0x80
    }
}
