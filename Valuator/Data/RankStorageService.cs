using StackExchange.Redis;
using System;
using Valuator.Data;

namespace Valuator.Data
{
  public class RankStorageService : IRankStorageService
  {
    private readonly IConnectionMultiplexer _redis;
    private readonly IValuatorRepository _valuatorRepository;

    public RankStorageService(
      IConnectionMultiplexer redis,
      IValuatorRepository valuatorRepository)
    {
      _redis = redis;
      _valuatorRepository = valuatorRepository;
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

      return _valuatorRepository.SaveRank(rankKey, rank);
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

      return _valuatorRepository.SaveSimilarity(similarityKey, similarity);
    }

    public double GetRank(string rankKey)
    {
      return _valuatorRepository.GetRank(rankKey);
    }

    public double GetSimilarity(string similarityKey)
    {
      return _valuatorRepository.GetSimilarity(similarityKey);
    }
  }
}
