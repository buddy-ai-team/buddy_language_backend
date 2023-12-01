using System.Reflection;

namespace BuddyLanguage.TelegramBot;

public static class ReflectionHelper
{
    public static DateTime GetBuildDate()
    {
        Version version = Assembly.GetExecutingAssembly().GetName().Version!;
        DateTime buildDate = new DateTime(2000, 1, 1)
            .AddDays(version.Build)
            .AddSeconds(version.Revision * 2);
        return buildDate;
    }
}
