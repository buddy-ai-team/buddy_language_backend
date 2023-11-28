using System.Web;
using FluentAssertions;

namespace BuddyLanguage.WebAPI.Test;

public class TmaTests
{
    [Fact]
    public void Telegram_userid_parsing_and_retriving_succeded()
    {
        const string expectedTgUserId = "279058397";

        // Arrange
        var authHeader = $"tma {GetTestInitData()}";

        // Act
        var telegramUserId = GetTgUserIdFromTma(authHeader);

        // Assert
        telegramUserId.Should().Be(expectedTgUserId);
    }

    private string GetTgUserIdFromTma(string tma)
    {
        var initDate = tma[4..];
        var queryParams = HttpUtility.ParseQueryString(initDate);

        string userJson = queryParams["user"]!;

        var (idStartsAtIndex, idEndsAtIndex) = (6, userJson.IndexOf(','));
        var telegramUserId = userJson[idStartsAtIndex..idEndsAtIndex];

        return telegramUserId;
    }

    private string GetTestInitData()
    {
        return
            "query_id=AAHdF6IQAAAAAN0XohDhrOrc&user=%7B%22id%22%3A279058397%2C%22first_name%22%3A%22Vladislav%22%2C%22last_name%22%3A%22Kibenko%22%2C%22username%22%3A%22vdkfrost%22%2C%22language_code%22%3A%22ru%22%2C%22is_premium%22%3Atrue%7D&auth_date=1662771648&hash=c501b71e775f74ce10e377dea85a7ea24ecd640b223ea86dfe453e0eaed2e2b2";
    }
}
