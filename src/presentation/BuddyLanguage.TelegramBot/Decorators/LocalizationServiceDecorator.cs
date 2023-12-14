using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace BuddyLanguage.TelegramBot.Decorators;

/// <summary>
/// Декоратор кеширования для сервиса ILocalizationService
/// Кеширует результаты обработки входящих запросов.
/// При повторном получении идентичного запроса будет возвращен результат,
/// хранящийся в cache.
/// </summary>
public class LocalizationServiceDecorator : ILocalizationService
{
    private readonly ILocalizationService _localizationService;

    private readonly IDistributedCache _cache;

    private readonly ILogger<LocalizationServiceDecorator> _logger;

    public LocalizationServiceDecorator(
        ILocalizationService localizationService,
        IDistributedCache cache,
        ILogger<LocalizationServiceDecorator> logger)
    {
        _localizationService = localizationService ??
                               throw new ArgumentNullException(nameof(localizationService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> GetText(
        LocalizableText text,
        Language preferredLanguage,
        CancellationToken cancellationToken)
    {
        if (text.Text == string.Empty)
        {
            _logger.LogWarning("Empty text sent to Localization Service");
            return string.Empty;
        }

        string key = KeyMaker(text.Text, preferredLanguage, "text");

        string? result = await _cache.GetStringAsync(key, cancellationToken);

        if (result is not null)
        {
            return result;
        }

        result = await _localizationService.GetText(text, preferredLanguage, cancellationToken);
        await _cache.SetStringAsync(key, result, cancellationToken);
        return result;
    }

    public async Task<byte[]> GetSpeech(
        LocalizableText text,
        Language preferredLanguage,
        CancellationToken cancellationToken)
    {
        if (text.Text == string.Empty)
        {
            _logger.LogWarning("Empty text sent to Localization Service");
            return Array.Empty<byte>();
        }

        string key = KeyMaker(text.Text, preferredLanguage, "speech");

        byte[]? result = await _cache.GetAsync(key, cancellationToken);

        if (result is not null)
        {
            return result;
        }

        result = await _localizationService.GetSpeech(text, preferredLanguage, cancellationToken);
        await _cache.SetAsync(key, result, cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод генерации уникального ключа запроса
    /// </summary>
    /// <param name="text">текст запроса</param>
    /// <param name="language">язык запроса</param>
    /// <param name="option">метод запроса</param>
    /// <returns>уникальный ключ</returns>
    private string KeyMaker(string text, Language language, string option)
    {
        return text + "_" + (int)language + "_" + option;
    }
}
