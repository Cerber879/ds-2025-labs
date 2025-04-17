namespace Valuator.Data
{
  public interface IValuatorRepository
  {
    bool DeleteAllRecords();

    bool SaveRank(string rankKey, double rank);
    bool SaveSimilarity(string similarityKey, double similarity);
    double GetRank(string rankKey);
    double GetSimilarity(string similarityKey);

    bool SaveText(string textKey, string text);
    string GetText(string textKey);

    bool CheckForPlagiarism(string text);
  }
}