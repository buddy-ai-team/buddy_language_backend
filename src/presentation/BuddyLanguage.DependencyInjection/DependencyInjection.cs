using BuddyLanguage.ChatGPTServiceLib;
using BuddyLanguage.Data.EntityFramework;
using BuddyLanguage.Data.EntityFramework.Repositories;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.OpenAIWhisperSpeechRecognitionService;
using BuddyLanguage.TextToSpeech;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.ChatGpt.EntityFrameworkCore.Extensions;
using OpenAI.Extensions;

namespace BuddyLanguage.DependencyInjection;

public static class BuddyLanguageDependencyInjection
{
    private static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        
        //Services
        //Azure TTS
        services.AddOptions<AzureTTSConfig>()
            .BindConfiguration("AzureTTSConfig")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        // Definition of database file name and connection of it as a service
        services.AddOptions<NpgsqlConnectionStringOptions>()
            .BindConfiguration("NpgsqlConnectionStringOptions")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        var config = configuration
            .GetSection("NpgsqlConnectionStringOptions")
            .Get<NpgsqlConnectionStringOptions>();
        
        // Подключение репозитория для работы с Ролями
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IRoleRepository, RoleRepositoryEf>();
        services.AddScoped<IUnitOfWork, UnitOfWorkEf>();
        services.AddScoped<RoleService>();

        

        services.AddDbContext<AppDbContext>(
            options => options.UseNpgsql(config.ConnectionString)
        );

        services.AddChatGptEntityFrameworkIntegration(
            op => op.UseNpgsql(config.ConnectionString)
        );

        services.AddScoped<IChatGPTService, ChatGPTService>(); 

        services.AddOpenAIService
        (settings =>
        {
            settings.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                              ?? throw new InvalidOperationException
                                  ("OPENAI_API_KEY environment variable is not set");
        });

        services.AddScoped<ISpeechRecognitionService, WhisperSpeechRecognitionService>();
        
        return services;
    }

    private static IServiceCollection AddDomainServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}