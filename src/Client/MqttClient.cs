using MqttLibNet.IO;
using MqttLibNet.Packets;
using MqttLibNet.Packets.Data;
using MqttLibNet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MqttLibNet.Client
{
    public class MqttClient
    {
        MqttStreamReaderWriter mqttStreamReaderWriter;
        IMqttStream mqttStream;
        MqttPublishQos0Service mqttPublishQos0Service;
        MqttPublishQos1ReceiverService mqttPublishQos1Service;
        MqttPublishQos1DispatchService mqttPublishQos1DispatchService;
        public async Task Handshake()
        {
            mqttStream = new MqttTcpStream();
            mqttStreamReaderWriter = new MqttStreamReaderWriter(mqttStream);
            ConnectData connectData = new ConnectData();
            connectData.CleanSession = true;
            connectData.ClientIdentifier = "TestClient";
            connectData.ProtocolName = "MQTT";
            connectData.ProtocolLevel = 4;
            connectData.KeepAliveMs = 30000;
            MqttHandshakeService mqttHandshakeService = new MqttHandshakeService(mqttStreamReaderWriter);
            mqttStreamReaderWriter.Read();
            var connack = await mqttHandshakeService.StartAsync(connectData);
            if(connack.ConnectReturnCode == ConnectReturnCode.ConnectionAccepted)
            {
                Console.WriteLine($"Conn done code {connack.ConnectReturnCode} session {connack.SessionPresent}");
                MqttMetronomeService mqttMetronomeService = new MqttMetronomeService(mqttStreamReaderWriter);
                mqttMetronomeService.Start();
                mqttPublishQos0Service = new MqttPublishQos0Service(mqttStreamReaderWriter);
                mqttPublishQos0Service.Start();
                mqttPublishQos1Service = new MqttPublishQos1ReceiverService(mqttStreamReaderWriter);
                mqttPublishQos1Service.Start();
                mqttPublishQos1DispatchService = new MqttPublishQos1DispatchService(mqttStreamReaderWriter);
                mqttPublishQos1DispatchService.Start();
            }
        }

        public async Task Subscribe()
        {
            SubscribeData subscribeData = new SubscribeData();
            subscribeData.PacketIdentifier = 10;
            subscribeData.Subscriptions = new List<(string TopicName, QosLevel Qos)>{("GPTKM", QosLevel.Qos0), ("GPTKM1", QosLevel.Qos1)};
            MqttSubscriptionService mqttSubscriptionService = new MqttSubscriptionService(mqttStreamReaderWriter);
            var subAckData = await mqttSubscriptionService.StartAsync(subscribeData);
            if(subAckData.PacketIdentifier == 10)
            {
                for(int i = 0; i <= subscribeData.Subscriptions.Count() - 1; ++i)
                {
                    var topicName = subscribeData.Subscriptions.ElementAt(i).TopicName;
                    var qosClient = subscribeData.Subscriptions.ElementAt(i).Qos;
                    var qosServer = subAckData.SubAckReturnCode.ElementAt(i);
                    Console.WriteLine($"TopicName- {topicName} qosClient {qosClient} qosServer {qosServer}");
                }
            }
            else
            {
                Console.WriteLine($"Sub failed received identifier is {subAckData.PacketIdentifier} passed is 10");
            }
        }

        public async Task Publish(string topicName, string message)
        {
            PublishData publishData = new PublishData();
            publishData.Message = message;
            publishData.TopicName = topicName;
            publishData.QosLevel = QosLevel.Qos0;
            await mqttPublishQos0Service.PushAsync(publishData);
        }

        public async Task PublishQos1(string topicName, string message)
        {
            PublishData publishData = new PublishData();
            publishData.Message = message;
            publishData.TopicName = topicName;
            publishData.QosLevel = QosLevel.Qos1;
            await mqttPublishQos1DispatchService.PublishAsync(publishData);
        }
    }
}
