using StackExchange.Redis;
using System.Linq;

namespace InfrastructureRedis
{
  public class RedisRepository : IRedisRepository
  {
    private readonly IConnectionMultiplexer _redis;
    private IDatabase Db => _redis.GetDatabase();
    private IServer Server => _redis.GetServer(_redis.GetEndPoints().First());

    public RedisRepository(IConnectionMultiplexer redis)
    {
      _redis = redis;
    }

    public bool DeleteAllRecords()
    {
      foreach (var key in Server.Keys())
      {
        Db.KeyDelete(key);
      }
      return true;
    }

    public bool SaveDoubleValue(string key, double value)
    {
      Db.StringSet(key, value);
      return true;
    }

    public bool SaveStringValue(string key, string value)
    {
      Db.StringSet(key, value);
      return true;
    }
    public double GetDoubleValue(string key)
    {
      var value = Db.StringGet(key);
      return value.IsNullOrEmpty ? 0 : (double)value;
    }

    public string GetStringValue(string key)
    {
      var value = Db.StringGet(key);
      return value.IsNullOrEmpty ? string.Empty : value.ToString();
    }

    public bool CheckForPlagiarism(string text)
    {
      var keys = Server.Keys(pattern: "TEXT-*");

      foreach (var key in keys)
      {
        var storedText = Db.StringGet(key);
        if (!storedText.IsNullOrEmpty && storedText == text)
          return true;
      }

      return false;
    }
  }
}
