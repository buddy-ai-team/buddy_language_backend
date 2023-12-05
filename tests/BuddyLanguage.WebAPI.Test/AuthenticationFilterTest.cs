using System.Web;
using BuddyLanguage.WebApi.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
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

        var builder = WebApplication.CreateBuilder();

        // Получение Id пользователя телеграм из строки заголовка
        int resultUserId = GetTgUserIdFromTma($"tma {GetTestInitData()}");

        // Инициализация ActionContext
        var actionContext = new ActionContext(httpContext, new RouteData { }, new ControllerActionDescriptor { });

        // Инициалиазация тестиру
        var filter = new AuthenticationFilter(_logger, builder.Configuration);

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
        const string start = "query_id=AAG4059_AAAAALjTn3-y9ZAg&user=%7B%22id%22%3A2141180856%2C%22first_name%22%3A%22%D0%9E%D0%BD%D0%B8%D1%89%D0%B5%D0%BD%D0%BA%D0%BE%20%D0%90%D0%BD%D0%BD%D0%B0%22%2C%22last_name%22%3A%22%22%2C%22username%22%3A%22Anna_Onis%22%2C%22language_code%22%3A%22ru%22%2C%22allows_write_to_pm%22%3Atrue%7D&";
        string date = "auth_date=" + "1701279684";
        const string end = "&hash=c85cdb29adad89e0f4635317bc1ae064f810d3e8227dd119c01bad0b9b406f69";
        return start + date + end;
    }
}
