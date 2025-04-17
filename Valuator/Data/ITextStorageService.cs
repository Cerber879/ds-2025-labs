namespace Valuator.Data
{
  public interface ITextStorageService
  {
    bool SaveText(string textKey, string text);
    string GetText(string textKey);
    bool CheckForPlagiarism(string text);
  }
}
