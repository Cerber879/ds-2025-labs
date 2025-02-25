using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Data;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IValuatorRepository _valuatorRepository;

    public IndexModel(ILogger<IndexModel> logger, IValuatorRepository valuatorRepository)
    {
        _logger = logger;
        _valuatorRepository = valuatorRepository;
    }

    public IActionResult OnPostDeleteAll()
    {
        _valuatorRepository.DeleteAllRecords();
        return RedirectToPage();
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string rankKey = "RANK-" + id;
        double rank = CalculateRank(text);
        _valuatorRepository.SaveRank(rankKey, rank);

        string similarityKey = "SIMILARITY-" + id;
        bool isDuplicate = _valuatorRepository.CheckForPlagiarism(text);
        double similarity = isDuplicate ? 1 : 0;
        _valuatorRepository.SaveSimilarity(similarityKey, similarity);

        string textKey = "TEXT-" + id;
        _valuatorRepository.SaveText(textKey, text);

        return Redirect($"summary?id={id}");
    }

    private double CalculateRank(string text)
    {
        int nonAlphabeticCount = text.Count(c => !Char.IsLetter(c));
        int totalCount = text.Length;

        return totalCount > 0 ? (double)nonAlphabeticCount / totalCount : 0;
    }

    // private bool IsAlphabetic(char c)
    // {
    //     return (char.IsLetter(c) &&
    //             ((c >= 'a' && c <= 'z') ||
    //               (c >= 'A' && c <= 'Z') ||
    //               (c >= 'а' && c <= 'я') ||
    //               (c >= 'А' && c <= 'Я')));
    // }

}
