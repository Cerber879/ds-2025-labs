using StackExchange.Redis;
using System;
using System.Linq;
using RankCalculator.Services;
using InfrastructureRedis;

namespace RankCalculator.Services
{
  public class TextStorageService : ITextStorageService
  {
    private readonly IRedisRepository _redisRepository;


    public TextStorageService(
      IRedisRepository redisRepository)
    {
      _redisRepository = redisRepository;
    }

    public string GetText(string textKey)
    {
      return _redisRepository.GetStringValue(textKey);
    }
  }
}
