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

        var configOptions = new ConfigurationOptions
        {
            EndPoints = { "127.0.0.1:6379" },
            Ssl = false
        };

        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configOptions));

        builder.Services.AddScoped<ITextStorageService, TextStorageService>();
        builder.Services.AddScoped<IRankStorageService, RankStorageService>();
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
