using InfluxDB.Client;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading;
using System.Threading.Tasks;

using Thomsen.HomeServer.Core.Mqtt;
using Thomsen.HomeServer.Core.Mqtt.Models;

namespace Thomsen.HomeServer.Service;

internal class Mqtt2InfluxRouter : IHostedService {
    private readonly ILogger _logger;
    private readonly IInfluxDBClient _influxDbClient;
    private readonly IReceiveSensorMqttClient _receiveSensorMqttClient;

    public Mqtt2InfluxRouter(ILogger<Mqtt2InfluxRouter> logger, IInfluxDBClient influxDbClient, IReceiveSensorMqttClient receiveSensorMqttClient) {
        _logger = logger;
        _receiveSensorMqttClient = receiveSensorMqttClient;
        _influxDbClient = influxDbClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("{Facility} starting", nameof(Mqtt2InfluxRouter));

        _receiveSensorMqttClient.ReceivedSensor += ReceiveSensorMqttClient_ReceivedSensor;

        await _receiveSensorMqttClient.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("{Facility} stopping", nameof(Mqtt2InfluxRouter));

        _receiveSensorMqttClient.ReceivedSensor -= ReceiveSensorMqttClient_ReceivedSensor;

        await _receiveSensorMqttClient.StopAsync(cancellationToken);
    }

    private async void ReceiveSensorMqttClient_ReceivedSensor(object sender, ISensorModel sensor) {
        _logger.LogInformation("{Facility} got valid sensor data ({type})", nameof(Mqtt2InfluxRouter), sensor.GetType().Name);
        _logger.LogTrace("{Facility}: Data: {model}", nameof(Mqtt2InfluxRouter), sensor);

        IWriteApiAsync writeApi = _influxDbClient.GetWriteApiAsync();
        await writeApi.WritePointAsync(sensor.ToPointData());
    }
}
