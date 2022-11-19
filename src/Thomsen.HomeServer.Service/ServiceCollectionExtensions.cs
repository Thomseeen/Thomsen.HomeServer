using Microsoft.Extensions.DependencyInjection;

using System;

namespace Thomsen.HomeServer.Service;

internal static class ServiceCollectionExtensions {
    public static IServiceCollection AddConfiguredServices(this IServiceCollection services, string[] serviceNames) {
        foreach (string serviceName in serviceNames) {
            services = serviceName switch {
                nameof(Tado2MqttService) => services.AddHostedService<Tado2MqttService>(),
                nameof(Mqtt2InfluxRouter) => services.AddHostedService<Mqtt2InfluxRouter>(),
                _ => throw new InvalidOperationException($"Unknown service {serviceName}")
            };
        }
        return services;
    }
}
