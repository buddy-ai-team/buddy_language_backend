﻿using BuddyLanguage.Domain.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = BuddyLanguage.Domain.Entities.User;

namespace BuddyLanguage.TelegramBot.Commands;

public class ResetTopicCommand : IBotCommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly BuddyService _buddyService;

    public ResetTopicCommand(
        ITelegramBotClient botClient,
        UserService userService,
        BuddyService buddyService)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _buddyService = buddyService ?? throw new ArgumentNullException(nameof(buddyService));
    }

    public string Command => "/reset";

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);
        if (update.Message is { From: not null } message)
        {
            var telegramId = message.From.Id.ToString();
            User user = await _userService.GetUserByTelegramId(telegramId, cancellationToken);
            await _buddyService.ResetTopic(user, cancellationToken);
            await _botClient.SendTextMessageAsync(
                message.Chat.Id, "Тема сброшена", cancellationToken: cancellationToken);
        }
    }
}
