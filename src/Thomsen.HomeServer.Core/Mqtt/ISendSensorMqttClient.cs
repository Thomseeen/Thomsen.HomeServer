using System.Threading.Tasks;

using Thomsen.HomeServer.Core.Mqtt.Models;

namespace Thomsen.HomeServer.Core.Mqtt;

public interface ISendSensorMqttClient {
    public Task SendSensorModelsAsync<T>(T[] sensors) where T : ISensorModel;
}