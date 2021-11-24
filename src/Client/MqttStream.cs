namespace MqttLibNet.Client
{
    public enum MqttStream
    {
        /// <summary>
        /// TCP is the underlying NW stream.
        /// </summary>
        Tcp=0,
        /// <summary>
        /// Websocket is the underlying stream.
        /// </summary>
        WebSocket=1
    }
}
