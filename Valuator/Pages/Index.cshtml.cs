using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Data;
using Valuator.RabbitMQ;
using InfrastructureRedis;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITextStorageService _textStorageService;
        private readonly ISimilarityStorageService _similarityStorageService;
        private readonly IRedisRepository _redisRepository;

        public string? ErrorMessage { get; private set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            ITextStorageService textStorageService,
            ISimilarityStorageService similarityStorageService,
            IRedisRepository redisRepository)
        {
            _logger = logger;
            _textStorageService = textStorageService;
            _similarityStorageService = similarityStorageService;
            _redisRepository = redisRepository;
        }

        public IActionResult OnPostDeleteAll()
        {
            try
            {
                _redisRepository.DeleteAllRecords();
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
                _similarityStorageService.SaveSimilarity(similarityKey, similarity);

                string textKey = "TEXT-" + id;
                _textStorageService.SaveText(textKey, text);

                await RabbitMQProducer.SendIdAsync(id);
                await RabbitMQProducer.PublishSimilarityCalculatedEvent(id, similarity);

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
