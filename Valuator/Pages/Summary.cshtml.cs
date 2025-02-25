using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Data;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IValuatorRepository _valuatorRepository;

    public SummaryModel(ILogger<SummaryModel> logger, IValuatorRepository valuatorRepository)
    {
        _logger = logger;
        _valuatorRepository = valuatorRepository;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        string rankKey = "RANK-" + id;
        string similarityKey = "SIMILARITY-" + id;

        Rank = _valuatorRepository.GetRank(rankKey);
        Similarity = _valuatorRepository.GetSimilarity(similarityKey);
    }
}
