using MqttLibNet.Packets;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace MqttLibNet.IO
{
    public class MqttStreamReaderWriter
    {
        private readonly IMqttStream mqttStream;
        private ConcurrentDictionary<IObserver<(byte[] Data, byte Flag)>, Func<(MqttControlPacketType packetType, byte flag), bool>> handlers;

        public MqttStreamReaderWriter(IMqttStream mqttStream)
        {
            this.mqttStream = mqttStream;
            this.handlers = new ConcurrentDictionary<IObserver<(byte[] Data, byte Flag)>, Func<(MqttControlPacketType packetType, byte flag), bool>>();
        }

        public void Read()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var controlPacketType = await FindControlPacketTypeAsync();
                        var variableLength = await FindPayloadAndVariableHeaderLength();
                        var matchingHandlers = handlers.Where(_ => _.Value(controlPacketType));
                        if (variableLength != 0)
                        {
                            try
                            {
                                var item = await mqttStream.ReadAsync(variableLength);
                                foreach (var handler in matchingHandlers)
                                {
                                    handler.Key.OnNext((item, controlPacketType.Flag));
                                }
                            }
                            catch
                            {
                                foreach (var handler in matchingHandlers)
                                {
                                    handler.Key.OnError(null);
                                }
                            }
                        }
                        else if (controlPacketType.PacketType == MqttControlPacketType.PingResp)
                        {
                            foreach (var handler in matchingHandlers)
                            {
                                handler.Key.OnNext((null, controlPacketType.Flag));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            });
        }

        public async Task WriteAsync(byte[] buffer)
        {
            await mqttStream.WriteAsync(buffer);
        }

        public bool Subscribe(
            IObserver<(byte[] Data, byte Flag)> packetHandler,
            Func<(MqttControlPacketType packetType, byte flag), bool> predicate)
        {
            return handlers.TryAdd(packetHandler, predicate);
        }

        private async Task<(MqttControlPacketType PacketType, byte Flag)> FindControlPacketTypeAsync()
        {
            var fixedHeader = await mqttStream.ReadByteAsync();
            int operand = 0b_11110000;
            int flagOperand = 0b_00001111;
            int packetType = fixedHeader & operand;
            int flag = fixedHeader & flagOperand;
            return ((MqttControlPacketType)packetType, (byte)flag);
        }

        private async Task<int> FindPayloadAndVariableHeaderLength()
        {
            bool continueRead = true;
            int multiplier = 1;
            int length = 0;
            while (continueRead)
            {
                if (multiplier > (128 * 128 * 128))
                {
                    throw new System.Exception("Variable length encoding for length of the control packet can be max 4 bytes");
                }

                var value = await mqttStream.ReadByteAsync();
                if (value < 128)
                {
                    continueRead = false;
                    length += (value * multiplier);
                }
                else
                {
                    length += (value - 128) * multiplier;
                }
                multiplier *= 128;
            }
            return length;
        }
    }
}
