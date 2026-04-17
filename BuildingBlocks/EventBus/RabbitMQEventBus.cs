using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BuildingBlocks.EventBus
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly IConnection Connection;
        private readonly IModel Channel;
        private readonly IConfiguration Configuration;

        public RabbitMQEventBus(IConfiguration configuration)
        {
            Configuration = configuration;

            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:Host"] ?? "localhost"
            };

            IConnection connection = null;

            int retries = 5;
            while (retries > 0)
            {
                try
                {
                    connection = factory.CreateConnection();
                    break;
                }
                catch
                {
                    retries--;
                    Console.WriteLine("RabbitMQ not ready, retrying...");
                    Thread.Sleep(5000);
                }
            }

            if (connection == null)
            {
                throw new Exception("Could not connect to RabbitMQ");
            }

            // v6 sync methods
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
        }

        public void Publish<T>(T @event)
        {
            var queueName = typeof(T).Name;

            // Declare queue (sync)
            Channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            // Publish message (sync)
            Channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body
            );
        }
    }
}
