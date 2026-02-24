using MetaExchange.Application.BestExecution;
using MetaExchange.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 8000;
});

// application services
builder.Services.AddTransient<IBestExecutionService, BestExecutionService>();

// configuration
builder.Services.Configure<VenuesOptions>(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
