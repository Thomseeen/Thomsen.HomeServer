using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using Thomsen.HomeServer.Core.Mqtt;
using Thomsen.HomeServer.Core.Mqtt.Models;
using Thomsen.HomeServer.Core.Tado;

namespace Thomsen.HomeServer.Service;

internal class Tado2MqttService : IHostedService {
    private readonly ILogger _logger;
    private readonly ITadoApiClient _tadoApiClient;
    private readonly ISendSensorMqttClient _sendSensorMqttClient;

    private readonly System.Timers.Timer _timer = new();

    public Tado2MqttService(ILogger<Tado2MqttService> logger, ITadoApiClient tadoApiClient, ISendSensorMqttClient sendSensorMqttClient) {
        _logger = logger;
        _tadoApiClient = tadoApiClient;
        _sendSensorMqttClient = sendSensorMqttClient;

        _timer.Interval = 60_000;
        _timer.Elapsed += Timer_Elapsed;
    }

    private async void Timer_Elapsed(object? sender, ElapsedEventArgs e) {
        _timer.Stop();

        await RunAsync();

        _timer.Start();
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        await RunAsync();

        _timer.Start();
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _timer.Stop();

        return Task.CompletedTask;
    }

    private async Task RunAsync() {
        TadoSensorModel[] sensors = await _tadoApiClient.GetSensorModelsAsync();

        await _sendSensorMqttClient.SendSensorModelsAsync(sensors);

        _logger.LogInformation("{Facility} got {count} sensors and sent them via MQTT", nameof(Tado2MqttService), sensors.Length);
        _logger.LogTrace("{Facility}: Data: {models}", nameof(Tado2MqttService), string.Join(';', sensors.Select(sensor => sensor.ToString())));
    }
}
