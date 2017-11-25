using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.ServiceBus
{
    public interface IServiceBusRepository
    {
        void InitQueueClient(string serviceBusAccessKeyName, string serviceBusAccessKey, string queue);
        Task SendAsync(object message);
    }
}
