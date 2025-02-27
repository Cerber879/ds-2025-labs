namespace Valuator.Data
{
  public interface IRankStorageService
  {
    bool SaveRank(string rankKey, double rank);
    bool SaveSimilarity(string similarityKey, double similarity);
    double GetRank(string rankKey);
    double GetSimilarity(string similarityKey);
  }
}
