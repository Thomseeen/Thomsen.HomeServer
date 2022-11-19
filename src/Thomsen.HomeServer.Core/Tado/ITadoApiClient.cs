using System.Threading.Tasks;

using Thomsen.HomeServer.Core.Mqtt.Models;

namespace Thomsen.HomeServer.Core.Tado;

public interface ITadoApiClient {
    public Task<TadoSensorModel[]> GetSensorModelsAsync();
}