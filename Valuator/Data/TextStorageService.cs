using StackExchange.Redis;
using System;
using System.Linq;

namespace Valuator.Data
{
  public class TextStorageService : ITextStorageService
  {
    private readonly IConnectionMultiplexer _redis;

    public TextStorageService(IConnectionMultiplexer redis)
    {
      _redis = redis;
    }

    public bool SaveText(string textKey, string text)
    {
      if (string.IsNullOrWhiteSpace(textKey))
      {
        throw new ArgumentException("Ключ к тексту не может быть пустым", nameof(textKey));
      }

      if (string.IsNullOrWhiteSpace(text))
      {
        throw new ArgumentException("Текст не может быть пустым", nameof(text));
      }

      var db = _redis.GetDatabase();
      db.StringSet(textKey, text);

      return true;
    }

    public bool CheckForPlagiarism(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
      {
        throw new ArgumentException("Текст не может быть пустым", nameof(text));
      }

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
