using Telegram.Bot;

namespace BuddyLanguage.Infrastructure.IntegrationTest;

public class TelegramTests
{
    [Fact]
    public async Task Telegram_bot_token_is_valid()
    {
        // Arrange
        var telegramToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
        if (telegramToken == null)
        {
            throw new InvalidOperationException("TELEGRAM_BOT_TOKEN environment variable is not set");
        }

        var botClient = new TelegramBotClient(telegramToken);

        // Act
        // Assert
        Assert.True(await botClient.TestApiAsync());
        
        
    }
}
