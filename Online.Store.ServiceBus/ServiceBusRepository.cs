using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Online.Store.ServiceBus
{
    public class ServiceBusRepository : IServiceBusRepository
    {
        IQueueClient _queueClient;
        IConfiguration _configuration;
        string _serviceBusNamespace;

        public ServiceBusRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _serviceBusNamespace = _configuration["ServiceBus:Namespace"];
        }


        public void InitQueueClient(string serviceBusAccessKeyName, string serviceBusAccessKey, string queue)
        {
            var connectionString =
                $"Endpoint=sb://{_serviceBusNamespace}" +
                $".servicebus.windows.net/;SharedAccessKeyName={serviceBusAccessKeyName};" +
                $"SharedAccessKey={serviceBusAccessKey}";

            _queueClient = new QueueClient(connectionString, queue);
        }

        public async Task SendAsync(object body)
        {
            var serializedBody = JsonConvert.SerializeObject(body);
            var message = new Message(Encoding.UTF8.GetBytes(serializedBody));
            await _queueClient.SendAsync(message);
        }
    }
}
