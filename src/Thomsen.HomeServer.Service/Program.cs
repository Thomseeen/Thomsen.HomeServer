using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Thomsen.HomeServer.Core;
using Thomsen.HomeServer.Core.InfluxDb;
using Thomsen.HomeServer.Core.Mqtt;
using Thomsen.HomeServer.Core.Tado;

namespace Thomsen.HomeServer.Service;

internal class Program {
    public static async Task<int> Main(string[] args) {
        string[] enabledServices = ParseArgs(args);

        IHostBuilder builder = Host.CreateDefaultBuilder(args);

        builder.UseSystemd();

        builder.ConfigureServices((cx, services) => {
            services.AddMqttClient();
            services.AddMqttClientOptions(cx.Configuration.GetSection("Mqtt").Get<MqttOptions>());

            services.AddInfluxDbClient(cx.Configuration.GetSection("InfluxDb").Get<InfluxDbOptions>());

            services.AddHttpClient(nameof(TadoApiClient));
            services.Configure<TadoOptions>(cx.Configuration.GetSection("Tado"));

            services.AddTransient<ITadoApiClient, TadoApiClient>();
            services.AddTransient<ISendSensorMqttClient, SendSensorMqttClient>();
            services.AddTransient<IReceiveSensorMqttClient, ReceiveSensorMqttClient>();

            services.AddSingleton<MqttSensorMapper>();

            services.AddConfiguredServices(enabledServices);
        });

        IHost host = builder.Build();

        Console.WriteLine($"--- {Assembly.GetEntryAssembly()?.GetName().Name} v{Assembly.GetEntryAssembly()?.GetName().Version} ---");

        await host.RunAsync();

        return 0;
    }

    private static string[] ParseArgs(string[] args) {
        List<string> enabledServices = new();
        for (int ii = 0; ii < args.Length; ii++) {
            switch (args[ii]) {
                case "-v":
                case "--version":
                    Console.WriteLine($"{Assembly.GetEntryAssembly()?.GetName().Name} v{Assembly.GetEntryAssembly()?.GetName().Version}");
                    Environment.Exit(0);
                    break;
                case "-s":
                case "--services":
                    while (ii < args.Length - 1 && !args[++ii].StartsWith('-')) {
                        enabledServices.Add(args[ii]);
                    }
                    break;
                default:
                    Console.WriteLine($"Unknown arg {args[ii]}");
                    Environment.Exit(-1);
                    break;
            }
        }

        // No services enabled explicitly, enable defaults
        if (enabledServices.Count == 0) {
            enabledServices.Add(nameof(Tado2MqttService));
            enabledServices.Add(nameof(Mqtt2InfluxRouter));
        }

        return enabledServices.ToArray();
    }
}
