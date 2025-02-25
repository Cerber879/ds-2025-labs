using StackExchange.Redis;

using Valuator.Models;
using System.Text.Json;

namespace Valuator.Data
{
  public class ValuatorRepository : IValuatorRepository
  {
    private readonly IConnectionMultiplexer _redis;

    public ValuatorRepository(IConnectionMultiplexer redis)
    {
      _redis = redis;
    }

    public bool SaveText(string textKey, string text)
    {
      if (string.IsNullOrWhiteSpace(textKey))
      {
        throw new ArgumentException("Ключ к тексту не может быть пустым", nameof(text));
      }

      if (string.IsNullOrWhiteSpace(text))
      {
        throw new ArgumentException("Текст не может быть пустым", nameof(text));
      }

      var db = _redis.GetDatabase();
      db.StringSet(textKey, text);

      return true;
    }

    public bool SaveRank(string rankKey, double rank)
    {
      if (string.IsNullOrWhiteSpace(rankKey))
      {
        throw new ArgumentException("Ключ к тексту не может быть пустым", nameof(rank));
      }

      if (double.IsNaN(rank) || (rank > 1 && rank < 0))
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
        throw new ArgumentException("Ключ к значнию сходства текста не может быть пустым", nameof(similarity));
      }

      if (double.IsNaN(similarity) || (similarity != 1 && similarity != 0))
      {
        throw new ArgumentException("Значние сходства текста должно быть 0 или 1", nameof(similarity));
      }

      var db = _redis.GetDatabase();
      db.StringSet(similarityKey, similarity);

      return true;
    }

    public bool CheckForPlagiarism(string text)
    {
      var db = _redis.GetDatabase();
      var server = _redis.GetServer(_redis.GetEndPoints().First());
      var keys = server.Keys(pattern: "TEXT-*").ToList();

      foreach (var key in keys)
      {
        var storedTextJson = db.StringGet(key);
        if (!storedTextJson.IsNullOrEmpty)
        {
          if (storedTextJson == text)
          {
            return true;
          }
        }
      }
      return false;
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

    public bool DeleteAllRecords()
    {
      var db = _redis.GetDatabase();
      var server = _redis.GetServer(_redis.GetEndPoints().First());
      var keys = server.Keys();

      foreach (var key in keys)
      {
        Console.WriteLine(key);
        db.KeyDelete(key);
      }

      return true;
    }

  }
}
