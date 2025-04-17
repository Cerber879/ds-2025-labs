using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Valuator.Data;

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
        await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
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
}
