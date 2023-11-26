using BuddyLanguage.WebApi.Filters;

namespace BuddyLanguage.WebApi.Extensions;

public static class WebApiIntegrationExtensions
{
    public static IServiceCollection AddWebApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //Swagger Setup
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        //Filters
        services.AddControllers(options =>
        {
            options.Filters.Add<CentralizedExceptionHandlingFilter>(order: 1);
        });

        return services;
    }

    public static WebApplication UseWebApi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        return app;
    }
}
