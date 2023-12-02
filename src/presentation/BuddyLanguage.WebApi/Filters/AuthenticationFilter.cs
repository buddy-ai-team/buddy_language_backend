using System.Text.Json;
using System.Web;
using BuddyLanguage.WebApi.Filters.AuthenticationData;
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
        // Получение строки заголовка Authorization
        string initDataHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(initDataHeader))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        try
        {
            // Преобразование строки заголовка Authorization к типу данных InitData
            var queryParams = HttpUtility.ParseQueryString(initDataHeader[4..]);
            InitData initData = new InitData
            {
                AuthDateRaw = int.Parse(queryParams["auth_date"]!),
                Hash = queryParams["hash"]!,
                QueryId = queryParams["query_id"]!,
                User = JsonSerializer.Deserialize<AuthUser>(queryParams["user"]!)!
            };

            if (!ValidateInitData(initData))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Запись идентификатора пользователя телеграм в поле TelegramUserId контекста запроса
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
