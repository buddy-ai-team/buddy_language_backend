﻿using BuddyLanguage.AzureServices;
using BuddyLanguage.ChatGPTServiceLib;
using BuddyLanguage.Data.EntityFramework;
using BuddyLanguage.Data.EntityFramework.Repositories;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.NAudioOggToWavConverter;
using BuddyLanguage.OpenAIWhisperSpeechRecognitionService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        // Подключение репозитория для работы с Ролями
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IRoleRepository, RoleRepositoryEf>();
        services.AddScoped<IWordEntityRepository, WordEntityRepositoryEf>();
        services.AddScoped<IUserRepository, UserRepositoryEf>();
        services.AddScoped<IUnitOfWork, UnitOfWorkEf>();

        // Can be replaced with Aspire.Microsoft.EntityFrameworkCore.SqlServer
        services.AddDbContext<AppDbContext>(
            options =>
            {
                options.UseSqlServer(
                        configuration.GetRequiredConnectionString("AZURE_SQL_CONNECTIONSTRING"),
                        builder =>
                        {
                            builder.UseAzureSqlDefaults();
                            builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                        })
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging();
            });

        services.AddChatGptEntityFrameworkIntegration(
            options =>
            {
                options.UseSqlServer(
                        configuration.GetRequiredConnectionString("AZURE_SQL_CONNECTIONSTRING"),
                        builder =>
                        {
                            builder.UseAzureSqlDefaults();
                            builder.MigrationsAssembly(typeof(InfrastructureMig).Assembly.FullName);
                        })
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging();
            });

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();

        services.AddOpenAIService(
            settings =>
            {
                settings.ApiKey = configuration.GetRequiredValue("OPENAI_API_KEY");
            });

        services.AddSingleton<INAudioOggToPcmConverter, NAudioOggToPcmConverter>();
        services.AddScoped<ISpeechRecognitionService, WhisperSpeechRecognitionService>();
        services.AddScoped<IPronunciationAssessmentService, PronunciationAssessmentService>();
        services.AddScoped<ITextToSpeech, AzureTextToSpeech>();
        services.AddScoped<IChatGPTService, ChatGPTService>();

        return services;
    }

    private static IServiceCollection AddDomainServices(
        this IServiceCollection services)
    {
        services.AddScoped<RoleService>();
        services.AddScoped<WordService>();
        services.AddScoped<UserService>();
        services.AddScoped<BuddyService>();

        return services;
    }
}
