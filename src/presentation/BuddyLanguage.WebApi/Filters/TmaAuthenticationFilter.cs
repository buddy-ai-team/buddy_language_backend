using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using BuddyLanguage.HttpModels.Responses;
using BuddyLanguage.Infrastructure;
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
public class TmaAuthenticationFilter : Attribute, IAuthorizationFilter, IFilterFactory
{
    private const int RequestValidTimeInMinutes = 60;
    private readonly ILogger<TmaAuthenticationFilter> _logger;
    private readonly string _botToken;

    public TmaAuthenticationFilter(
        ILogger<TmaAuthenticationFilter> logger,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _botToken = configuration.GetRequiredValue("BotConfiguration:Token");
    }

    public bool IsReusable => false;

    /// <summary>
    /// Проверка соответствия полученного hash расчётному и запрос "свежий" и задает TelegramUserId в контексте запроса
    /// </summary>
    /// <param name="context"></param>
    /// <remarks>
    /// Строка заголовка Authorization начинается на tma затем пробел затем состоит из параметров в формате
    /// </remarks>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Получение строки заголовка Authorization
        string initDataHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(initDataHeader))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        try
        {
            // Преобразование строки заголовка Authorization к коллекции параметров
            var listOfParams = ParseHeaderToListOfData(initDataHeader);

            // Получение пользователя из заголовка
            string userJson = listOfParams["user"];
            InitData initData = new InitData()
            {
                AuthDateRaw = int.Parse(listOfParams["auth_date"]),
                Hash = listOfParams["hash"],
                QueryId = listOfParams["query_id"],
                User = JsonSerializer.Deserialize<AuthUser>(userJson)!
            };

            // Проверка соответствия полученного hash расчётному и
            // запрос "свежий"
            if (!ValidateInitData(listOfParams))
            {
                context.Result = new ObjectResult(
                    new ErrorResponse("Ошибка во время валидации данных: Неверный hash"))
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            // Запись идентификатора пользователя телеграм в поле TelegramUserId контекста запроса
            context.HttpContext.Items["TelegramUserId"] = initData.User.Id;
        }
        catch (Exception e)
        {
            context.Result = new ObjectResult(new ErrorResponse(e.Message))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }
    }

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var log = serviceProvider.GetRequiredService<ILogger<TmaAuthenticationFilter>>();
        var conf = serviceProvider.GetRequiredService<IConfiguration>();

        return new TmaAuthenticationFilter(log, conf);
    }

    // Проверить корректность подписи полученных данных
    private bool ValidateInitData(IDictionary<string, string> pairs)
    {
        string dataCheckString = BuildDataCheckString(pairs);
        string signature = Sign(dataCheckString, _botToken);

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
        using var skHmac = new HMACSHA256("WebAppData"u8.ToArray());
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] skHmacBytes = skHmac.ComputeHash(keyBytes);

        using var impHmac = new HMACSHA256(skHmacBytes);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        byte[] impHmacBytes = impHmac.ComputeHash(payloadBytes);
        return BitConverter.ToString(impHmacBytes).Replace("-", string.Empty).ToLower();
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
