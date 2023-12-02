using System.Web;
using BuddyLanguage.WebApi.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.WebAPI.Test;

public class AuthenticationFilterTest
{
    private readonly ILogger<AuthenticationFilter> _logger;

    public AuthenticationFilterTest()
    {
        _logger = new LoggerFactory().CreateLogger<AuthenticationFilter>();
    }

    [Fact]
    public Task User_telegram_Id_recieved_from_Authorization_header()
    {
        // Arrange
        // Инициализация HttpContext c тестовой информацией в заголовке Authorization
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] =
            $"tma {GetTestInitData()}";

        // Получение Id пользователя телеграм из строки заголовка
        int resultUserId = GetTgUserIdFromTma($"tma {GetTestInitData()}");

        // Инициализация ActionContext
        var actionContext = new ActionContext(httpContext, new RouteData { }, new ControllerActionDescriptor { });

        // Инициалиазация тестиру
        var filter = new AuthenticationFilter(_logger);

        // Act
        filter.OnAuthorization(new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>()));

        // Assert
        httpContext.Items["TelegramUserId"].Should().Be(resultUserId);
        return Task.CompletedTask;
    }

    // Получить телеграм Id пользователя из строки по расположению данных
    private int GetTgUserIdFromTma(string tma)
    {
        var initDate = tma[4..];
        var queryParams = HttpUtility.ParseQueryString(initDate);

        string userJson = queryParams["user"]!;

        var (idStartsAtIndex, idEndsAtIndex) = (6, userJson.IndexOf(','));
        var telegramUserId = userJson[idStartsAtIndex..idEndsAtIndex];

        return int.Parse(telegramUserId);
    }

    // Получить пример строки заголовка Authorization
    private string GetTestInitData()
    {
        return
            "query_id=AAHdF6IQAAAAAN0XohDhrOrc&user=%7B%22id%22%3A279058397%2C%22first_name%22%3A%22Vladislav%22%2C%22last_name%22%3A%22Kibenko%22%2C%22username%22%3A%22vdkfrost%22%2C%22language_code%22%3A%22ru%22%2C%22is_premium%22%3Atrue%7D&auth_date=1662771648&hash=c501b71e775f74ce10e377dea85a7ea24ecd640b223ea86dfe453e0eaed2e2b2";
    }
}
