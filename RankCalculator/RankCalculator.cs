using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RankCalculator.Services;
using System.Text.Json;

namespace Consumer;

public class RankCalculator
{
    private const string QueueName = "valuator.processing.rank";
    private readonly IRankStorageService _rankStorageService;
    private readonly ITextStorageService _textStorageService;
    private readonly IChannel _channel;

    public RankCalculator(
        IRankStorageService rankStorageService,
        ITextStorageService textStorageService,
        IChannel channel
    )
    {
        _rankStorageService = rankStorageService;
        _textStorageService = textStorageService;
        _channel = channel;
    }

    public async Task StartAsync()
    {
        await DeclareTopologyAsync();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += HandleMessageAsync;

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine("Consumer started. Press Enter to exit.");
        Console.ReadLine();
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        Console.WriteLine("HandleMessageAsync started.");

        string id = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        string textKey = "TEXT-" + id;
        string text = _textStorageService.GetText(textKey);

        if (string.IsNullOrWhiteSpace(text))
        {
            await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            return;
        }

        string rankKey = "RANK-" + id;
        double rank = CalculateRank(text);
        _rankStorageService.SaveRank(rankKey, rank);

        await PublishRankCalculatedEvent(id, rank);

        await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);

        Console.WriteLine("HandleMessageAsync ended.");
    }

    private async Task DeclareTopologyAsync()
    {
        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );
    }

    private double CalculateRank(string text)
    {
        int nonAlphabeticCount = text.Count(c => !char.IsLetter(c));
        int totalCount = text.Length;
        return totalCount > 0 ? (double)nonAlphabeticCount / totalCount : 0;
    }

    private async Task PublishRankCalculatedEvent(string id, double rank)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var exchangeName = "events";
        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout);

        var message = JsonSerializer.Serialize(new { Event = "RankCalculated", Id = id, Rank = rank });
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: exchangeName, routingKey: "", body: body);
    }

}
