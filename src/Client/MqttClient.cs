using MqttLibNet.IO;
using MqttLibNet.Packets;
using MqttLibNet.Packets.Data;
using MqttLibNet.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MqttLibNet.Client
{
    public class MqttClient
    {
        private const short SubscribePacketIdentifier = 10;
        private readonly MqttStreamReaderWriter mqttStreamReaderWriter;
        private readonly MqttHandshakeService mqttHandshakeService;
        private readonly MqttQos0Service mqttPublishQos0Service;
        private readonly MqttPublishQos1ReceiverService mqttPublishQos1Service;
        private readonly MqttPublishQos1DispatchService mqttPublishQos1DispatchService;
        private readonly MqttSubscriptionService mqttSubscriptionService;
        private readonly MqttMetronomeService mqttMetronomeService;
        private MqttClientConfiguration connectionContext = new MqttClientConfiguration();

        /// <summary>
        /// CTOR.
        /// </summary>
        /// <param name="mqttStream"></param>
        /// <param name="mqttStreamReaderWriter"></param>
        /// <param name="mqttPublishQos0Service"></param>
        /// <param name="mqttPublishQos1Service"></param>
        /// <param name="mqttPublishQos1DispatchService"></param>
        public MqttClient(
            IMqttStream mqttStream,
            MqttStreamReaderWriter mqttStreamReaderWriter,
            MqttHandshakeService mqttHandshakeService,
            MqttQos0Service mqttPublishQos0Service,
            MqttPublishQos1ReceiverService mqttPublishQos1Service,
            MqttPublishQos1DispatchService mqttPublishQos1DispatchService,
            MqttSubscriptionService mqttSubscriptionService,
            MqttMetronomeService mqttMetronomeService)
        {
            this.mqttStreamReaderWriter = mqttStreamReaderWriter;
            this.mqttHandshakeService = mqttHandshakeService;
            this.mqttPublishQos0Service = mqttPublishQos0Service;
            this.mqttPublishQos1Service = mqttPublishQos1Service;
            this.mqttPublishQos1DispatchService = mqttPublishQos1DispatchService;
            this.mqttSubscriptionService = mqttSubscriptionService;
            this.mqttMetronomeService = mqttMetronomeService;
        }

        /// <summary>
        /// This connect method holds the orchestration logic to connect to the broker.
        /// </summary>
        /// <param name="mqttClientConfiguration"></param>
        /// <returns></returns>
        public async Task<MqttConnectResult> ConnectAsync(MqttClientConfiguration mqttClientConfiguration)
        {
            var connackData = await Handshake(mqttClientConfiguration);
            MqttConnectResult mqttConnectResult = new MqttConnectResult();
            if (connackData.ConnectReturnCode == ConnectReturnCode.ConnectionAccepted)
            {
                var subscriptionResponse = await Subscribe(mqttClientConfiguration.TopicConfiguration);
                mqttConnectResult.TopicConfiguration = subscriptionResponse;
                connectionContext = mqttClientConfiguration;
                connectionContext.CleanSession = connackData.SessionPresent;
                connectionContext.TopicConfiguration = subscriptionResponse;
            }
            mqttConnectResult.ConnectReturnCode = connackData.ConnectReturnCode;
            mqttConnectResult.SessionPresent = connackData.SessionPresent;
            return mqttConnectResult;
        }

        /// <summary>
        /// Simply publish the data on the topic with given qos level.
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="message"></param>
        /// <param name="qosLevel"></param>
        /// <returns></returns>
        public async Task Publish(string topicName, string message, QosLevel qosLevel)
        {
            PublishData publishData = new PublishData();
            publishData.Message = message;
            publishData.TopicName = topicName;
            publishData.QosLevel = qosLevel;
            if (qosLevel == QosLevel.Qos0)
            {
                await mqttPublishQos0Service.PublishAsync(publishData);
            }
            else
            {
                await mqttPublishQos1DispatchService.PublishAsync(publishData);
            }
        }
       

        private async Task<ConnackData> Handshake(MqttClientConfiguration mqttClientConfiguration)
        {
            ConnectData connectData = new ConnectData(mqttClientConfiguration);
            mqttStreamReaderWriter.Read();
            var connack = await mqttHandshakeService.StartAsync(connectData);
            if (connack.ConnectReturnCode == ConnectReturnCode.ConnectionAccepted)
            {
                mqttMetronomeService.Start();
                mqttPublishQos0Service.Start();
                mqttPublishQos1Service.Start();
                mqttPublishQos1DispatchService.Start();
            }
            return connack;
        }

        private async Task<IEnumerable<MqttTopicConfiguration>> Subscribe(IEnumerable<MqttTopicConfiguration> topicConfiguration)
        {
            SubscribeData subscribeData = new SubscribeData(topicConfiguration, SubscribePacketIdentifier);
            List<MqttTopicConfiguration> subscribedTopics = new List<MqttTopicConfiguration>();
            var subAckData = await mqttSubscriptionService.StartAsync(subscribeData);
            if (subAckData.PacketIdentifier == SubscribePacketIdentifier)
            {
                for (int i = 0; i <= subscribeData.Subscriptions.Count() - 1; ++i)
                {
                    var topicName = subscribeData.Subscriptions.ElementAt(i).TopicName;
                    var qos = subAckData.SubAckReturnCode.ElementAt(i);
                    subscribedTopics.Add(new MqttTopicConfiguration(topicName, (QosLevel)qos));
                }
            }
            return subscribedTopics;
        }
    }
}
