using StackExchange.Redis;
using System;
using System.Linq;
using Valuator.Data;
using InfrastructureRedis;

namespace Valuator.Data
{
  public class TextStorageService : ITextStorageService
  {
    private readonly IRedisRepository _redisRepository;


    public TextStorageService(
      IRedisRepository redisRepository)
    {
      _redisRepository = redisRepository;
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

      return _redisRepository.SaveStringValue(textKey, text);
    }

    public bool CheckForPlagiarism(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
      {
        throw new ArgumentException("Текст не может быть пустым", nameof(text));
      }

      return _redisRepository.CheckForPlagiarism(text);
    }
  }
}
