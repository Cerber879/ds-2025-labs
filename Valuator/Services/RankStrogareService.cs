using StackExchange.Redis;
using System;
using Valuator.Data;
using InfrastructureRedis;

namespace Valuator.Data
{
  public class RankStorageService : IRankStorageService
  {
    private readonly IRedisRepository _redisRepository;

    public RankStorageService(
      IRedisRepository redisRepository)
    {
      _redisRepository = redisRepository;
    }

    public double GetRank(string rankKey)
    {
      return _redisRepository.GetDoubleValue(rankKey);
    }
  }
}
