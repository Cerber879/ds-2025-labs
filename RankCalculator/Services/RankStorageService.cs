using StackExchange.Redis;
using System;
using RankCalculator.Services;
using InfrastructureRedis;

namespace RankCalculator.Services
{
  public class RankStorageService : IRankStorageService
  {
    private readonly IRedisRepository _redisRepository;

    public RankStorageService(
      IRedisRepository redisRepository)
    {
      _redisRepository = redisRepository;
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

      return _redisRepository.SaveDoubleValue(rankKey, rank);
    }

    public double GetRank(string rankKey)
    {
      return _redisRepository.GetDoubleValue(rankKey);
    }

  }
}
