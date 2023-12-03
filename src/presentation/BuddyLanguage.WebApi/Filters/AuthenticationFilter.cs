using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using BuddyLanguage.WebApi.Filters.AuthenticationData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuddyLanguage.WebApi.Filters;

/// <summary>
/// Фильтр аутентификации.
/// Проверяет соответствие полученного hash ключа расчётному.
/// Записывает в поле HttpContext.Items["TelegramUserId"] телеграм ID пользователя,
/// сделавшего запрос
/// </summary>
public class AuthenticationFilter : Attribute, IAuthorizationFilter
{
    private const int RequestValidTimeInMinutes = 5;
    private readonly ILogger<AuthenticationFilter> _logger;

    public AuthenticationFilter(
        ILogger<AuthenticationFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Получение токен телеграм бота из переменных среды
        string? botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")!;
        if (botToken is null)
        {
            throw new ArgumentNullException(nameof(botToken));
        }

        // Получение строки заголовка Authorization
        string initDataHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(initDataHeader))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        try
        {
            // Преобразование строки заголовка Authorization к коллекции параметров
            var listOfParams = ParseHeaderToListOfData(initDataHeader);

            // Получение пользователя из заголовка
            InitData initData = new InitData()
            {
                AuthDateRaw = int.Parse(listOfParams["auth_date"]),
                Hash = listOfParams["hash"],
                QueryId = listOfParams["query_id"],
                User = JsonSerializer.Deserialize<AuthUser>(listOfParams["user"])!
            };

            // Проверка соответствия полученного hash расчётному и
            // запрос "свежий"
            if (!ValidateInitData(listOfParams, botToken))
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

    // Проверить корректность подписи полученных данных
    private bool ValidateInitData(IDictionary<string, string> pairs, string botToken)
    {
        string dataCheckString = BuildDataCheckString(pairs);
        string signature = Sign(dataCheckString, botToken);

        bool result = signature.Equals(pairs["hash"], StringComparison.OrdinalIgnoreCase);
        return result;
    }

    // Создать строку для генерации hash
    private string BuildDataCheckString(IDictionary<string, string> pairs)
    {
        List<string> result = new();
        foreach (var value in pairs)
        {
            if (value.Key != "hash")
            {
                result.Add(value.Key + "=" + value.Value);
            }
        }

        return string.Join("\n", result);
    }

    // Сгенерировать hash на основе: кодовой фраз, токена телеграм бота и полученной строки данных
    private string Sign(string payload, string key)
    {
        using (var skHmac = new HMACSHA256(Encoding.UTF8.GetBytes("WebAppData")))
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] skHmacBytes = skHmac.ComputeHash(keyBytes);

            using (var impHmac = new HMACSHA256(skHmacBytes))
            {
                byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
                byte[] impHmacBytes = impHmac.ComputeHash(payloadBytes);
                return BitConverter.ToString(impHmacBytes).Replace("-", string.Empty).ToLower();
            }
        }
    }

    // Проверить "свежесть" запроса
    private bool IsAuthDateValid(DateTime authDate)
    {
        var currentTime = DateTime.UtcNow;
        return (currentTime - authDate).TotalMinutes <= RequestValidTimeInMinutes;
    }

    // Преобразовать строку заголовка в коллекцию параметров
    private IDictionary<string, string> ParseHeaderToListOfData(string header)
    {
        var queryParams = HttpUtility.ParseQueryString(header[4..]);

        var listOfParams = new SortedDictionary<string, string>();
        foreach (var param in queryParams)
        {
            listOfParams.Add(param.ToString()!, queryParams[param.ToString()]!);
        }

        return listOfParams;
    }
}
