using System.Threading;
using System.Threading.Tasks;

namespace Thomsen.HomeServer.Core.Mqtt;

public interface IReceiveSensorMqttClient {
    public event ReceiveSensorMqttClient.ReceivedSensorHandler? ReceivedSensor;

    public Task StartAsync(CancellationToken cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken);
}