namespace Valuator.Data
{
  public interface ITextStorageService
  {
    bool SaveText(string textKey, string text);
    bool CheckForPlagiarism(string text);
  }
}
