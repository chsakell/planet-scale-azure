using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Online.Store.Core.DTOs;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Online.Store.WebJob
{
    class Program
    {
        static string _serviceBusConnString = string.Empty;
        const string QueueName = "orders";
        static IQueueClient queueClient;
        static IOrderService _orderService;

        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();

            Startup startup = new Startup();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            _orderService = serviceProvider.GetService<IOrderService>();

            Console.WriteLine("=============== START RECEIVING ORDERS ================");

            _serviceBusConnString =
                $"Endpoint=sb://{startup.Configuration["ServiceBus:Namespace"]}.servicebus.windows.net/;SharedAccessKeyName={startup.Configuration["ServiceBus:ReadAccessKeyName"]};SharedAccessKey={startup.Configuration["ServiceBus:ReadAccessKey"]}";

            StartReceivingOrders(startup.Configuration["ServiceBus:Queue"]);

            Console.Read();

            queueClient.CloseAsync();
        }

        static void StartReceivingOrders(string queue)
        {
            queueClient = new QueueClient(_serviceBusConnString, queue);

            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            // Register the function that will process orders
            queueClient.RegisterMessageHandler(ProcessOrderAsync, messageHandlerOptions);
        }

        static async Task ProcessOrderAsync(Message message, CancellationToken token)
        {
            // Process the message
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            Order order = JsonConvert.DeserializeObject<Order>(Encoding.UTF8.GetString(message.Body));

            await _orderService.AddOrderAsync(order);

            Console.WriteLine("ORDER: " + order.Id + " has been submitted successfully");

            // Complete the message so that it is not received again.
            // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been Closed, you may chose to not call CompleteAsync() or AbandonAsync() etc. calls 
            // to avoid unnecessary exceptions.
        }

        // Use this Handler to look at the exceptions received on the MessagePump
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
