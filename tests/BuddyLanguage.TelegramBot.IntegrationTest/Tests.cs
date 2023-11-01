using FluentAssertions;

namespace BuddyLanguage.TelegramBot.IntegrationTest;

public class Tests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public Tests(CustomWebApplicationFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    [Fact]
    public void Server_started()
    {
        //примерно 600 мс на первичное создание клиента
        using HttpClient client = _factory.CreateClient(); //внутри процесса (Kestrel не запускается)
        // TODO вызов GetMe у бота
        client.Should().NotBeNull();
    }
}
