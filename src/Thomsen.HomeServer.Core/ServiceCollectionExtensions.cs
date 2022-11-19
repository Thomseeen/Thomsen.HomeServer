using InfluxDB.Client;

using Microsoft.Extensions.DependencyInjection;

using MQTTnet;
using MQTTnet.Client;

using Thomsen.HomeServer.Core.InfluxDb;
using Thomsen.HomeServer.Core.Mqtt;

namespace Thomsen.HomeServer.Core;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddMqttClient(this IServiceCollection services) {
        return services.AddTransient(services => new MqttFactory().CreateMqttClient());
    }

    public static IServiceCollection AddMqttClientOptions(this IServiceCollection services, MqttOptions options) {
        return services.AddTransient(services => new MqttClientOptionsBuilder()
            .WithCredentials(options.User, options.Password)
            .WithTcpServer(options.Address, options.Port)
            .WithTls(new MqttClientOptionsBuilderTlsParameters() {
                UseTls = options.UseTls,
                AllowUntrustedCertificates = true,
                CertificateValidationHandler = (args) => true
            })
            .WithCleanSession()
            .Build());
    }

    public static IServiceCollection AddInfluxDbClient(this IServiceCollection services, InfluxDbOptions options) {
        return services.AddTransient<IInfluxDBClient>(services => InfluxDBClientFactory.Create(
            InfluxDBClientOptions.Builder.CreateNew()
                .Url(options.Url)
                .AuthenticateToken(options.Token)
                .Org(options.Org)
                .Bucket(options.Bucket)
                .VerifySsl(false)
                .Build()));
    }
}