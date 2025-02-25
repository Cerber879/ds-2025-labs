using StackExchange.Redis;
using Valuator.Data;
using DotNetEnv;

namespace Valuator;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();

        var builder = WebApplication.CreateBuilder(args);

        string? redisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD");

        if (string.IsNullOrEmpty(redisPassword))
        {
            // Обработка случая, если переменная окружения не установлена
            throw new InvalidOperationException("REDIS_PASSWORD не найдена в переменных окружения.");
        }

        // Создание конфигурации для подключения с паролем
        var configOptions = new ConfigurationOptions
        {
            EndPoints = { "127.0.0.1:6379" },
            Password = redisPassword,
            Ssl = false
        };

        // Добавить Redis с конфигурацией
        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configOptions));
        builder.Services.AddScoped<IValuatorRepository, ValuatorRepository>();

        // Add services to the container.
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}
