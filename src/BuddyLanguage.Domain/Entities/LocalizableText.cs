using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Domain.Entities;

public class LocalizableText
{
    private readonly ILogger<LocalizableText> _logger;
    private string _text;

    protected LocalizableText(string text, ILogger<LocalizableText> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _text = text;
    }

    public string Text
    {
        get => _text;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogWarning("Empty text used to initialize LocalizableText");
                _text = string.Empty;
            }
            else
            {
                _text = value;
            }
        }
    }
}
