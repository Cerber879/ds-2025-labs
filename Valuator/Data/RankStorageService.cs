using StackExchange.Redis;
using System;

namespace Valuator.Data
{
  public class RankStorageService : IRankStorageService
  {
    private readonly IConnectionMultiplexer _redis;

    public RankStorageService(IConnectionMultiplexer redis)
    {
      _redis = redis;
    }

    public bool SaveRank(string rankKey, double rank)
    {
      if (string.IsNullOrWhiteSpace(rankKey))
      {
        throw new ArgumentException("Ключ не может быть пустым", nameof(rankKey));
      }

      if (rank < 0 || rank > 1)
      {
        throw new ArgumentException("Ранг должен быть от 0 до 1", nameof(rank));
      }

      var db = _redis.GetDatabase();
      db.StringSet(rankKey, rank);

      return true;
    }

    public bool SaveSimilarity(string similarityKey, double similarity)
    {
      if (string.IsNullOrWhiteSpace(similarityKey))
      {
        throw new ArgumentException("Ключ не может быть пустым", nameof(similarityKey));
      }

      if (similarity != 0 && similarity != 1)
      {
        throw new ArgumentException("Значение сходства должно быть 0 или 1", nameof(similarity));
      }

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
  }
}
