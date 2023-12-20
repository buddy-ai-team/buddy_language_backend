using BuddyLanguage.TelegramBot.Commands;

namespace BuddyLanguage.TelegramBot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBotCommandHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<IBotCommandHandler>()
            .AddClasses(classes => classes.AssignableTo<IBotCommandHandler>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
