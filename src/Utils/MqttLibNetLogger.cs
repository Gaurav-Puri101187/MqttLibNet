using Microsoft.Extensions.Logging;

namespace MqttLibNet.Utils
{
    public class MqttLibNetLogger<T>
    {
        private readonly ILogger<T> logger;

        public MqttLibNetLogger(ILogger<T> logger = null)
        {
            this.logger = logger;
        }

        public void LogDebug(string messageTemplate, params object[] parameters)
        {
            this.logger?.LogDebug(messageTemplate, parameters);
        }

        public void LogInformation(string messageTemplate, params object[] parameters)
        {
            this.logger?.LogInformation(messageTemplate, parameters);
        }

        public void LogWarning(string messageTemplate, params object[] parameters)
        {
            this.logger?.LogWarning(messageTemplate, parameters);
        }

        public void LogError(string messageTemplate, params object[] parameters)
        {
            this.logger?.LogError(messageTemplate, parameters);
        }
    }
}
