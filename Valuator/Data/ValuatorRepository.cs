using StackExchange.Redis;

namespace Valuator.Data
{
  public class ValuatorRepository : IValuatorRepository
  {
    private readonly IConnectionMultiplexer _redis;

    public ValuatorRepository
    (
      IConnectionMultiplexer redis
    )
    {
      _redis = redis;
    }

    public bool DeleteAllRecords()
    {
      var db = _redis.GetDatabase();
      var server = _redis.GetServer(_redis.GetEndPoints().First());
      var keys = server.Keys();

      foreach (var key in keys)
      {
        db.KeyDelete(key);
      }

      return true;
    }
  }
}
