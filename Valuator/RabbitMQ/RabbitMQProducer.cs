using System.Text;
using RabbitMQ.Client;
using System.Text.Json;

namespace Valuator.RabbitMQ;

public static class RabbitMQProducer
{
    private const string ExchangeName = "valuator.processing.rank";
    private const string QueueName = "valuator.processing.rank";

    public static async Task SendIdAsync(string id)
    {
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        await using IConnection connection = await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Direct
        );

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        await channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: ""
        );

        byte[] messageData = Encoding.UTF8.GetBytes(id);
        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: "",
            mandatory: false,
            body: messageData
        );

        await connection.CloseAsync();
    }

    public static async Task PublishSimilarityCalculatedEvent(string id, double similarity)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var exchangeName = "events";
        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout);

        var message = JsonSerializer.Serialize(new { Event = "SimilarityCalculated", Id = id, Similarity = similarity });
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: exchangeName, routingKey: "", body: body);
    }

}
