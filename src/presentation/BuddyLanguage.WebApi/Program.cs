using BuddyLanguage.Infrastructure;
using BuddyLanguage.WebApi.Filters;

var builder = WebApplication.CreateBuilder(args);

//Swagger Setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Domain and Infrastructure services
builder.Services.AddApplicationServices(builder.Configuration);

//Filters
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CentralizedExceptionHandlingFilter>(order: 1);
});

var app = builder.Build();

//Swagger Build
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapGet("/lifecheck", () => "OK");

app.Run();
