using StackExchange.Redis;
using System;
using System.Linq;
using Valuator.Data;

namespace Valuator.Data
{
  public class TextStorageService : ITextStorageService
  {
    private readonly IConnectionMultiplexer _redis;
    private readonly IValuatorRepository _valuatorRepository;


    public TextStorageService(
      IConnectionMultiplexer redis,
      IValuatorRepository valuatorRepository)
    {
      _redis = redis;
      _valuatorRepository = valuatorRepository;
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

      return _valuatorRepository.SaveText(textKey, text);
    }

    public string GetText(string textKey) => _valuatorRepository.GetText(textKey);

    public bool CheckForPlagiarism(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
      {
        throw new ArgumentException("Текст не может быть пустым", nameof(text));
      }

      return _valuatorRepository.CheckForPlagiarism(text);
    }
  }
}
