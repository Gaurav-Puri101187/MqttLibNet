using MqttLibNet.IO;
using MqttLibNet.Packets;
using System;
using System.Threading.Tasks;

namespace MqttLibNet.Services
{
    public class MqttMetronomeService : IObserver<(byte[] Data, byte Flag)>
    {
        private readonly int timeMs;
        private readonly MqttStreamReaderWriter mqttStreamReaderWriter;
        private static ulong count;
        public MqttMetronomeService(
            MqttStreamReaderWriter mqttStreamReaderWriter)
        {
            timeMs = 10000;
            this.mqttStreamReaderWriter = mqttStreamReaderWriter;
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
            Console.WriteLine($"Ping response message - {DateTime.UtcNow} - Total ping messages so far {++count}");
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
