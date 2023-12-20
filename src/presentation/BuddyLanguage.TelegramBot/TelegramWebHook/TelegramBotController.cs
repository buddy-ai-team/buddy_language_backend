using BuddyLanguage.TelegramBot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.TelegramWebHook;

[ApiController]
[Route("api/tg-webhook")]
public class TelegramBotController : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBotFilter]
    public IActionResult Post(
        [FromBody] Update update,
        [FromServices] TelegramBotService telegramBotService,
        CancellationToken cancellationToken)
    {
        // TODO: Add cancellation token
        _ = telegramBotService.HandleUpdate(update, default);
        return Ok();
    }
}
