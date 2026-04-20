using BuildingBlocks.EventBus;
using BuildingBlocks.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PaymentService.Infrastructure.Messaging
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly IEventBus EventBus;
        private readonly IConfiguration Configuration;

        public OrderCreatedConsumer(IEventBus eventBus, IConfiguration configuration)
        {
            EventBus = eventBus;
            Configuration = configuration;
        }

        /// <summary>
        /// Background service that consumes OrderCreated events from RabbitMQ and processes payment asynchronously
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = Configuration["RabbitMQ:Host"] ?? "localhost"
            };

            IConnection connection = null;

            int retryCount = 10;

            while (retryCount > 0)
            {
                try
                {
                    connection = factory.CreateConnection();
                    Console.WriteLine("Connected to RabbitMQ");
                    break;
                }
                catch
                {
                    retryCount--;
                    Console.WriteLine("RabbitMQ not ready, retrying in 5s...");
                    await Task.Delay(5000);
                }
            }

            if (connection == null)
                throw new Exception("Could not connect to RabbitMQ");

            var channel = connection.CreateModel();

            var queueName = nameof(OrderCreatedEvent);

            channel.QueueDeclare(queueName, false, false, false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                await ProcessPayment(orderEvent);

                Console.WriteLine($"Payment processed for Order: {orderEvent.OrderId}");
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        private async Task ProcessPayment(OrderCreatedEvent order)
        {
            await Task.Delay(1000);

            Console.WriteLine($"Payment processed for Order: {order.OrderId}");
        }
    }
}
