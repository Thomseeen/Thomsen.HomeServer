using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Reflection;
using System.Threading.Tasks;

using Thomsen.HomeServer.Core;
using Thomsen.HomeServer.Core.InfluxDb;

namespace Thomsen.HomeServer.Api;

internal class Program {
    public static async Task<int> Main(string[] args) {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSystemd();

        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddInfluxDbClient(builder.Configuration.GetSection("InfluxDb").Get<InfluxDbOptions>());
        builder.Services.AddTransient<IInfluxDbAdapter, InfluxDbAdapter>();

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.ConfigureApi(builder.Configuration.GetSection("Api").Get<ApiOptions>());

        Console.WriteLine($"--- {Assembly.GetEntryAssembly()?.GetName().Name} v{Assembly.GetEntryAssembly()?.GetName().Version} ---");

        await app.RunAsync();

        return 0;
    }
}