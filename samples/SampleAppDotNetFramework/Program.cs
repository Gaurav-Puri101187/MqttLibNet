using Microsoft.Extensions.DependencyInjection;
using MqttLibNet.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleAppDotNetFramework
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create a ServiceCollection 
            var container = new ServiceCollection();
            // Setup broker configurations
            MqttBrokerConfiguration mqttBrokerConfiguration = new MqttBrokerConfiguration();
            mqttBrokerConfiguration.Broker = "broker.emqx.io";
            mqttBrokerConfiguration.Port = 8883;
            mqttBrokerConfiguration.ReadWriteTimeoutMs = 60000;
            mqttBrokerConfiguration.SSLEnabled = true;
            // Setup MqttClient DI
            var sp = container.AddMqttClientWithTcp(mqttBrokerConfiguration).BuildServiceProvider();
            // Fetch the Client from SP
            var client = sp.GetService<MqttClient>();
            // Connect with the client.
            MqttClientConfiguration mqttClientConfiguration = new MqttClientConfiguration();
            mqttClientConfiguration.CleanSession = true;
            mqttClientConfiguration.ClientIdentifier = "TestClient";
            mqttClientConfiguration.KeepAliveMs = 20000;
            mqttClientConfiguration.ProtocolLevel = 4;
            List<MqttTopicConfiguration> topics = new List<MqttTopicConfiguration>()
            {
                new MqttTopicConfiguration("GPTKM", MqttLibNet.Packets.QosLevel.Qos0),
                new MqttTopicConfiguration("GPTKM1", MqttLibNet.Packets.QosLevel.Qos1)
            };
            mqttClientConfiguration.TopicConfiguration = topics;
            var res = await client.ConnectAsync(mqttClientConfiguration);
            if (res.ConnectReturnCode == MqttLibNet.Packets.ConnectReturnCode.ConnectionAccepted)
            {
                Console.WriteLine($"Connection Successful with session {res.SessionPresent}");
                foreach (var item in res.TopicConfiguration)
                {
                    Console.WriteLine($"Subs are Name:{item.Name} Level:{item.Level}");
                }

            }
            // Infinite loop to push messages to the client.
            int count = 0;
            while (true)
            {
                var msg = ++count;
                await Task.Delay(15000);
                await client.Publish("TestGPTKM", msg.ToString(), MqttLibNet.Packets.QosLevel.Qos1);
            }
        }
    }
}
