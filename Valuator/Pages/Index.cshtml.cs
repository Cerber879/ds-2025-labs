using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Data;
using Valuator.Models;
using Valuator.RabbitMQ;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITextStorageService _textStorageService;
        private readonly IRankStorageService _rankStorageService;
        private readonly IValuatorRepository _valuatorRepository;

        public string? ErrorMessage { get; private set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            ITextStorageService textStorageService,
            IRankStorageService rankStorageService,
            IValuatorRepository valuatorRepository)
        {
            _logger = logger;
            _textStorageService = textStorageService;
            _rankStorageService = rankStorageService;
            _valuatorRepository = valuatorRepository;
        }

        public IActionResult OnPostDeleteAll()
        {
            try
            {
                _valuatorRepository.DeleteAllRecords();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении всех записей.");
                ErrorMessage = "Ошибка при удалении всех записей.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPost(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    ErrorMessage = "Текст не должен быть пустым.";
                    return Page();
                }

                _logger.LogDebug(text);

                string id = Guid.NewGuid().ToString();

                string similarityKey = "SIMILARITY-" + id;
                bool isDuplicate = _textStorageService.CheckForPlagiarism(text);
                double similarity = isDuplicate ? 1 : 0;
                _rankStorageService.SaveSimilarity(similarityKey, similarity);

                string textKey = "TEXT-" + id;
                _textStorageService.SaveText(textKey, text);

                await RabbitMQProducer.SendIdAsync(id);

                return Redirect($"summary?id={id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке текста.");
                ErrorMessage = "Произошла ошибка при обработке текста. Попробуйте ещё раз.";
                return Page();
            }
        }
    }
}
