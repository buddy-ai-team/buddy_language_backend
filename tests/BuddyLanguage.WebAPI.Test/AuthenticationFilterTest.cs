using BuddyLanguage.WebApi.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Moq;

namespace BuddyLanguage.WebAPI.Test;

public class AuthenticationFilterTest
{
    private readonly ILogger<AuthenticationFilter> _logger;

    public AuthenticationFilterTest()
    {
        _logger = new LoggerFactory().CreateLogger<AuthenticationFilter>();
    }

    [Fact]
    public Task I_do_not_know_how_to_name_it()
    {
        // Arrange
        var httpContextMock = new Mock<HttpContext>();
        httpContextMock
            .Setup(a => a.Request.Headers["Authorization"])
            .Returns("query_id=AAHdF6IQAAAAAN0XohDhrOrc&user=%7B%22id%22%3A279058397%2C%22first_name%22%3A%22Vladislav%22%2C%22last_name%22%3A%22Kibenko%22%2C%22username%22%3A%22vdkfrost%22%2C%22language_code%22%3A%22ru%22%2C%22is_premium%22%3Atrue%7D&auth_date=1662771648&hash=c501b71e775f74ce10e377dea85a7ea24ecd640b223ea86dfe453e0eaed2e2b2");

        ActionContext fakeActionContext =
            new ActionContext(httpContextMock.Object,
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        AuthorizationFilterContext fakeAuthFilterContext =
            new AuthorizationFilterContext(fakeActionContext,
                new List<IFilterMetadata>());

        var filter = new AuthenticationFilter(_logger);

        // Act
        filter.OnAuthorization(fakeAuthFilterContext);

        // Assert
        httpContextMock.Object.Items["TelegramUserId"].Should().Be("279058397");
        return Task.CompletedTask;
    }
}
