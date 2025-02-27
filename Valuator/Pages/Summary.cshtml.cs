using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Data;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private readonly IRankStorageService _rankStorageService;

        public SummaryModel(
            ILogger<SummaryModel> logger,
            IRankStorageService rankStorageService)
        {
            _logger = logger;
            _rankStorageService = rankStorageService;
        }

        public string? ErrorMessage { get; private set; }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    ErrorMessage = "Некорректный идентификатор.";
                    return;
                }

                _logger.LogDebug(id);

                string rankKey = "RANK-" + id;
                string similarityKey = "SIMILARITY-" + id;

                Rank = _rankStorageService.GetRank(rankKey);
                Similarity = _rankStorageService.GetSimilarity(similarityKey);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных.");
                ErrorMessage = "Произошла ошибка при получении данных. Попробуйте ещё раз.";
            }
        }
    }
}
