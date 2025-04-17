using StackExchange.Redis;
using Valuator.Data;
using RabbitMQ.Client;
using Consumer;

class Program
{
  public static async Task Main(string[] args)
  {
    var redis = await ConnectionMultiplexer.ConnectAsync("localhost");
    IValuatorRepository repository = new ValuatorRepository(redis);

    var rankStorageService = new RankStorageService(redis, repository);
    var textStorageService = new TextStorageService(redis, repository);

    var factory = new ConnectionFactory() { HostName = "localhost" };
    var connection = await factory.CreateConnectionAsync();
    var channel = await connection.CreateChannelAsync();

    var calculator = new RankCalculator(rankStorageService, textStorageService, channel);
    await calculator.StartAsync();
  }
}
