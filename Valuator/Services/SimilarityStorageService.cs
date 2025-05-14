using StackExchange.Redis;
using System;
using Valuator.Data;
using InfrastructureRedis;

namespace Valuator.Data
{
  public class SimilarityStorageService : ISimilarityStorageService
  {
    private readonly IRedisRepository _redisRepository;

    public SimilarityStorageService(
      IRedisRepository redisRepository)
    {
      _redisRepository = redisRepository;
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

      return _redisRepository.SaveDoubleValue(similarityKey, similarity);
    }

    public double GetSimilarity(string similarityKey)
    {
      return _redisRepository.GetDoubleValue(similarityKey);
    }
  }
}
