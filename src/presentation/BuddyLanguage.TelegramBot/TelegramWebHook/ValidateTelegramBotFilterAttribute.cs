using BuddyLanguage.TelegramBot.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace BuddyLanguage.TelegramBot.TelegramWebHook;

/// <summary>
/// Check for "X-Telegram-Bot-Api-Secret-Token"
/// Read more: <see href="https://core.telegram.org/bots/api#setwebhook"/> "secret_token"
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ValidateTelegramBotFilterAttribute : TypeFilterAttribute
{
    public ValidateTelegramBotFilterAttribute()
        : base(typeof(ValidateTelegramBotFilter))
    {
    }

    private class ValidateTelegramBotFilter : IActionFilter
    {
        private readonly string _secretToken;

        public ValidateTelegramBotFilter(IOptions<BotConfiguration> options)
        {
            ArgumentNullException.ThrowIfNull(options);
            _secretToken = options.Value.WebHookSecret;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsValidRequest(context.HttpContext.Request))
            {
                context.Result = new ObjectResult("\"X-Telegram-Bot-Api-Secret-Token\" is invalid")
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                };
            }
        }

        private bool IsValidRequest(HttpRequest request)
        {
            var isSecretTokenProvided =
                request.Headers.TryGetValue(
                    "X-Telegram-Bot-Api-Secret-Token",
                    out var secretTokenHeader);
            if (!isSecretTokenProvided)
            {
                return false;
            }

            return string.Equals(secretTokenHeader, _secretToken, StringComparison.Ordinal);
        }
    }
}
