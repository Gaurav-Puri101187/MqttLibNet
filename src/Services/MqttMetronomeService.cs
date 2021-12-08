using MqttLibNet.IO;
using MqttLibNet.Packets;
using MqttLibNet.Packets.Handlers;
using MqttLibNet.Utils;
using System;
using System.Threading.Tasks;

namespace MqttLibNet.Services
{
    public class MqttMetronomeService : IObserver<(byte[] Data, byte Flag)>
    {
        private readonly int timeMs;
        private readonly MqttStreamReaderWriter mqttStreamReaderWriter;
        private readonly MqttLibNetLogger<MqttMetronomeService> logger;
        private static ulong count;
        public MqttMetronomeService(
            MqttStreamReaderWriter mqttStreamReaderWriter,
            MqttLibNetLogger<MqttMetronomeService> logger)
        {
            timeMs = 10000;
            this.mqttStreamReaderWriter = mqttStreamReaderWriter;
            this.logger = logger;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext((byte[] Data, byte Flag) pingResp)
        {
            this.logger.LogDebug("Ping response message {pingTime} ping message no is {pingMessageNo}", DateTime.UtcNow, ++count);
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    PingReq pingReq = new PingReq();
                    var pingPacket = pingReq.Serialize(null);
                    await mqttStreamReaderWriter.WriteAsync(pingPacket);
                    await Task.Delay(timeMs);
                }
            });
            mqttStreamReaderWriter.Subscribe(this, _ => _.packetType == MqttControlPacketType.PingResp);
        }
    }
}
