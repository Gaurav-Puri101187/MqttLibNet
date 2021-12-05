using Microsoft.Extensions.DependencyInjection;
using MqttLibNet.IO;
using MqttLibNet.Services;
using System;

namespace MqttLibNet.Client
{
    /// <summary>
    /// Dependency Container Extension.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddMqttClientWithTcp(
            this IServiceCollection serviceCollection, 
            MqttBrokerConfiguration mqttBrokerConfiguration)
        {
            serviceCollection.AddSingleton<IMqttStream>(new MqttTcpStream(
                mqttBrokerConfiguration.Broker,
                mqttBrokerConfiguration.Port,
                mqttBrokerConfiguration.SSLEnabled,
                mqttBrokerConfiguration.ReadWriteTimeoutMs));
            serviceCollection.AddSingleton<MqttStreamReaderWriter>();
            serviceCollection.AddSingleton<MqttHandshakeService>();
            serviceCollection.AddSingleton<MqttPublishQos1DispatchService>();
            serviceCollection.AddSingleton<MqttPublishQos1ReceiverService>();
            serviceCollection.AddSingleton<MqttQos0Service>();
            serviceCollection.AddSingleton<MqttMetronomeService>();
            serviceCollection.AddSingleton<MqttSubscriptionService>();
            serviceCollection.AddSingleton<MqttClient>();
            return serviceCollection;
        }

        public static void AddMqttClientWithWebSocket(this IServiceCollection serviceProvider)
        {
            throw new NotImplementedException();
        }
    }
}
