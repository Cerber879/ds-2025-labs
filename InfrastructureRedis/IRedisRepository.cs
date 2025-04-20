namespace InfrastructureRedis
{
  public interface IRedisRepository
  {
    bool DeleteAllRecords();
    public bool SaveStringValue(string key, string value);
    bool SaveDoubleValue(string key, double value);
    double GetDoubleValue(string key);
    string GetStringValue(string key);

    bool CheckForPlagiarism(string text);
  }
}