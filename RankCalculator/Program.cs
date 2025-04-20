using StackExchange.Redis;
using RankCalculator.Services;
using InfrastructureRedis;
using RabbitMQ.Client;
using Consumer;

class Program
{
  public static async Task Main(string[] args)
  {
    var configOptions = new ConfigurationOptions
    {
      EndPoints = { "127.0.0.1:6379" },
      Ssl = false,
      AbortOnConnectFail = false
    };

    IConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(configOptions);
    IRedisRepository redisRepository = new RedisRepository(redis);
    IRankStorageService rankStorageService = new RankStorageService(redisRepository);
    ITextStorageService textStorageService = new TextStorageService(redisRepository);

    var factory = new ConnectionFactory() { HostName = "localhost" };
    var connection = await factory.CreateConnectionAsync();
    var channel = await connection.CreateChannelAsync();

    var calculator = new Consumer.RankCalculator(rankStorageService, textStorageService, channel);
    await calculator.StartAsync();
  }
}
