using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Online.Store.Core.DTOs;
using Online.Store.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Azure.Services
{
    public class ServiceBusService : IServiceBusService
    {
        IServiceBusRepository _serviceBusRepository;
        IConfiguration _configuration;

        public ServiceBusService(IServiceBusRepository serviceBusRepository, IConfiguration configuration)
        {
            _serviceBusRepository = serviceBusRepository;
            _configuration = configuration;
        }

        public async Task SubmitOrderAsync(Order order)
        {
            _serviceBusRepository.InitQueueClient(_configuration["ServiceBus:WriteAccessKeyName"],
                _configuration["ServiceBus:WriteAccessKey"], _configuration["ServiceBus:Queue"]);

            await _serviceBusRepository.SendAsync(order);
        }
    }

    public interface IServiceBusService
    {
        Task SubmitOrderAsync(Order order);
    }

}
