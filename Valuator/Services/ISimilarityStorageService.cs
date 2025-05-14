namespace Valuator.Data
{
  public interface ISimilarityStorageService
  {
    bool SaveSimilarity(string similarityKey, double similarity);
    double GetSimilarity(string similarityKey);
  }
}
