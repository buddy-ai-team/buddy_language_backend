using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuddyLanguage.WebApi.Filters;

public class AuthenticationFilter : Attribute, IAuthorizationFilter
{
    private readonly ILogger<AuthenticationFilter> _logger;

    public AuthenticationFilter(
        ILogger<AuthenticationFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var initDataHeader = context.HttpContext.Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(initDataHeader))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        try
        {
            var initData = JsonSerializer.Deserialize<InitData>(initDataHeader!);
            if (initData == null || !ValidateInitData(initData))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.HttpContext.Items["TelegramUserId"] = initData.User.Id;
        }
        catch (Exception)
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private bool ValidateInitData(InitData initData)
    {
        // Implement your validation logic here, for example, checking the signature
        // You may need to convert the logic from your Go code to C#
        return true; // return true if valid, false otherwise
    }
}
