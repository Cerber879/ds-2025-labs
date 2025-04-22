using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

class Program
{
  private const string exchangeName = "events";
  public static async Task Main()
  {
    var factory = new ConnectionFactory { HostName = "localhost" };
    var connection = await factory.CreateConnectionAsync();
    var channel = await connection.CreateChannelAsync();

    await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout);

    var queueName = await channel.QueueDeclareAsync().ContinueWith(t => t.Result.QueueName);
    await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: "");

    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.ReceivedAsync += async (sender, ea) =>
    {
      var body = ea.Body.ToArray();
      var message = Encoding.UTF8.GetString(body);
      var json = JsonSerializer.Deserialize<JsonElement>(message);

      var eventType = json.GetProperty("Event").GetString();
      var id = json.GetProperty("Id").GetString();

      Console.WriteLine($"[Event] Type: {eventType}, Id: {id}");

      if (eventType == "RankCalculated")
        Console.WriteLine($"Rank: {json.GetProperty("Rank").GetDouble()}");
      else if (eventType == "SimilarityCalculated")
        Console.WriteLine($"Similarity: {json.GetProperty("Similarity").GetDouble()}");

      await Task.CompletedTask;
    };

    await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

    Console.WriteLine("EventsLogger is listening...");
    Console.ReadLine();
  }
}
