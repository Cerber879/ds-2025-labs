using Valuator.Models;

namespace Valuator.Data
{
  public interface IValuatorRepository
  {
    bool CheckForPlagiarism(string text);

    bool SaveText(string textKey, string text);
    bool SaveRank(string rankKey, double rank);
    bool SaveSimilarity(string similarityKey, double similarity);

    double GetRank(string rankKey);
    double GetSimilarity(string similarityKey);
    bool DeleteAllRecords();

  }
}