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

    public bool SaveRank(string rankKey, double rank)
    {
      var db = _redis.GetDatabase();
      db.StringSet(rankKey, rank);

      return true;
    }

    public bool SaveSimilarity(string similarityKey, double similarity)
    {
      var db = _redis.GetDatabase();
      db.StringSet(similarityKey, similarity);

      return true;
    }

    public double GetRank(string rankKey)
    {
      var db = _redis.GetDatabase();
      return db.StringGet(rankKey).IsNullOrEmpty ? 0 : (double)db.StringGet(rankKey);
    }

    public double GetSimilarity(string similarityKey)
    {
      var db = _redis.GetDatabase();
      return db.StringGet(similarityKey).IsNullOrEmpty ? 0 : (double)db.StringGet(similarityKey);
    }

    public bool SaveText(string textKey, string text)
    {
      var db = _redis.GetDatabase();
      db.StringSet(textKey, text);

      return true;
    }

    public string GetText(string textKey)
    {
      var db = _redis.GetDatabase();
      return db.StringGet(textKey).IsNullOrEmpty ? "" : (string)db.StringGet(textKey);
    }

    public bool CheckForPlagiarism(string text)
    {
      var db = _redis.GetDatabase();
      var server = _redis.GetServer(_redis.GetEndPoints().First());
      var keys = server.Keys(pattern: "TEXT-*").ToList();

      foreach (var key in keys)
      {
        var storedText = db.StringGet(key);
        if (!storedText.IsNullOrEmpty && storedText == text)
        {
          return true;
        }
      }
      return false;
    }
  }
}
