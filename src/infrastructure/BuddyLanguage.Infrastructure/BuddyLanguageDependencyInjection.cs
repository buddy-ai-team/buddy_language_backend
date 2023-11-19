using BuddyLanguage.AzureServices;
using BuddyLanguage.ChatGPTServiceLib;
using BuddyLanguage.Data.EntityFramework;
using BuddyLanguage.Data.EntityFramework.Repositories;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.OpenAIWhisperSpeechRecognitionService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.ChatGpt.EntityFrameworkCore;
using OpenAI.ChatGpt.EntityFrameworkCore.Extensions;
using OpenAI.Extensions;

namespace BuddyLanguage.Infrastructure;

public static class BuddyLanguageDependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
        services.AddDomainServices();

        return services;
    }

    private static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //Services
        //Azure TTS
        services.AddOptions<AzureConfig>()
            .BindConfiguration("AzureTTSConfig")
            .ValidateDataAnnotations()
            ; //.ValidateOnStart()

        // Definition of database file name and connection of it as a service
        services.AddOptions<SqlConnectionStringOptions>()
            .BindConfiguration("AzureSqlConnectionStringOptions")
            .ValidateDataAnnotations()
            ; //.ValidateOnStart()

        var config = configuration
            .GetRequiredSection("AzureSqlConnectionStringOptions")
            .Get<SqlConnectionStringOptions>();

        if (config is null || string.IsNullOrEmpty(config.ConnectionString))
        {
            throw new InvalidOperationException("AzureSqlConnectionStringOptions is missing or invalid in configuration.");
        }

        // Подключение репозитория для работы с Ролями
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IRoleRepository, RoleRepositoryEf>();
        services.AddScoped<IWordEntityRepository, WordEntityRepositoryEf>();
        services.AddScoped<IUserRepository, UserRepositoryEf>();
        services.AddScoped<IUnitOfWork, UnitOfWorkEf>();

        services.AddDbContext<AppDbContext>(
            options => options.UseSqlServer(
                config.ConnectionString,
                builder =>
                {
                    builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                }));

        services.AddChatGptEntityFrameworkIntegration(
            options => options.UseSqlServer(
                config.ConnectionString,
                builder =>
                {
                    builder.MigrationsAssembly(typeof(Stub).Assembly.FullName);
                }));

        services.AddScoped<IChatGPTService, ChatGPTService>();

        services.AddOpenAIService(
        settings =>
        {
            settings.ApiKey = configuration["OPENAI_API_KEY"] ?? throw new InvalidOperationException(
                                  "OPENAI_API_KEY environment variable is not set");
        });

        services.AddScoped<ISpeechRecognitionService, WhisperSpeechRecognitionService>();
        services.AddScoped<ITextToSpeech, AzureTextToSpeech>();

        services.AddScoped<IChatGPTService, ChatGPTService>();

        return services;
    }

    private static IServiceCollection AddDomainServices(
        this IServiceCollection services)
    {
        services.AddScoped<RoleService>();
        services.AddScoped<IWordService, WordService>();
        services.AddScoped<UserService>();
        services.AddScoped<BuddyService>();

        return services;
    }
}
