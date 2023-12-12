using BuddyLanguage.TelegramBot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.TelegramWebHook;

[Route("api/tg-webhook")]
public class TelegramBotController : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBotFilter]
    public async Task<IActionResult> Post(
        [FromBody] Update update,
        [FromServices] TelegramBotService telegramBotService,
        CancellationToken cancellationToken)
    {
        await telegramBotService.HandleUpdate(update, cancellationToken);
        return Ok();
    }
}
