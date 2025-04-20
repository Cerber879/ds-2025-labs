namespace RankCalculator.Services
{
  public interface IRankStorageService
  {
    bool SaveRank(string rankKey, double rank);
    double GetRank(string rankKey);
  }
}
