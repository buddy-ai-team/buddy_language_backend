using System.Security.Cryptography;
using System.Text;
using BuddyLanguage.WebApi.Filters.AuthenticationData;

namespace BuddyLanguage.WebApi.Filters;

public static class TelegramAuthHelper
{
    public static bool ValidateInitData(InitData initData, string botToken)
    {
        string dataCheckString = BuildDataCheckString(initData);
        string secretKey = ComputeHmacSha256Hash(botToken, "WebAppData");
        string signature = ComputeHmacSha256Hash(dataCheckString, secretKey);

        return signature.Equals(initData.Hash, StringComparison.OrdinalIgnoreCase) && IsAuthDateValid(initData.AuthDate);
    }

    private static string BuildDataCheckString(InitData initData)
    {
        var properties = initData.GetType().GetProperties()
            .Where(p => p.GetValue(initData) != null && p.Name != "Hash")
            .OrderBy(p => p.Name)
            .Select(p => $"{p.Name.ToLower()}={p.GetValue(initData)}");

        return string.Join("\n", properties);
    }

    private static string ComputeHmacSha256Hash(string message, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }

    private static bool IsAuthDateValid(DateTime authDate)
    {
        var currentTime = DateTime.UtcNow;

        // Example: consider data valid if it's not older than 5 minutes
        return (currentTime - authDate).TotalMinutes <= 5;
    }
}
