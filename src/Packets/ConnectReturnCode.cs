namespace MqttLibNet.Packets
{
    public enum ConnectReturnCode
    {
        /// <summary>
        /// Connection Accepted.
        /// </summary>
        ConnectionAccepted = 0x00,
        /// <summary>
        /// Unacceptable protocol version
        /// </summary>
        ConnectionRefused_UnAcceptableProtocolVersion = 0x01,
        /// <summary>
        /// Identifier rejected
        /// </summary>
        ConnectionRefused_IdentifierRejected = 0x02,
        /// <summary>
        /// Server unavailable
        /// </summary>
        ConnectionRefused_ServerUnavailable = 0x03,
        /// <summary>
        /// Bad user name or password
        /// </summary>
        ConnectionRefused_BadUserNamePwd = 0x04,
        /// <summary>
        /// Not authorized
        /// </summary>
        ConnectionRefused_UnAuthorized = 0x05,

    }
}
