using BuddyLanguage.WebApi.Filters;
using BuddyLanguage.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//Domain and Infrastructure services
builder.Services.AddServiceCollection(builder.Configuration);

//Filters
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CentralizedExceptionHandlingFilter>(order: 1);
});

var app = builder.Build();

app.MapControllers();

app.Run();