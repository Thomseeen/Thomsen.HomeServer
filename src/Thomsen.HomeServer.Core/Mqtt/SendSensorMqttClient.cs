using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using MQTTnet.Client;

using System;
using System.Linq;
using System.Threading.Tasks;

using Thomsen.HomeServer.Core.Mqtt.Models;

namespace Thomsen.HomeServer.Core.Mqtt;

public class SendSensorMqttClient : ISendSensorMqttClient {
    private readonly ILogger _logger;

    private readonly string _topLevelTopic;

    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttClientOptions;

    private readonly MqttSensorMapper _mqttSensorMapper;

    public SendSensorMqttClient(ILogger<SendSensorMqttClient> logger, IConfiguration configuration, IMqttClient mqttClient, MqttClientOptions mqttClientOptions, MqttSensorMapper mqttSensorMapper) {
        _logger = logger;
        _topLevelTopic = configuration.GetValue<string>("General:MqttTopLevelTopic");
        _mqttClient = mqttClient;
        _mqttClientOptions = mqttClientOptions;
        _mqttSensorMapper = mqttSensorMapper;

        _mqttClientOptions.ClientId = $"{nameof(SendSensorMqttClient)}-{Guid.NewGuid()}";
    }

    public async Task SendSensorModelsAsync<T>(T[] sensors) where T : ISensorModel {
        await ConnectAsync();

        await Task.WhenAll(sensors.Select(async sensor => {
            return await _mqttClient.PublishAsync(_mqttSensorMapper.ToMqttMsg(sensor));
        }));

        await DisconnectAsync();
    }

    private async Task ConnectAsync() {
        _logger.LogDebug("{Facility} connecting", nameof(SendSensorMqttClient));

        _mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;

        await _mqttClient.ConnectAsync(_mqttClientOptions);
    }

    private async Task DisconnectAsync() {
        _logger.LogDebug("{Facility} disconnecting", nameof(SendSensorMqttClient));

        _mqttClient.ConnectedAsync -= MqttClient_ConnectedAsync;

        await _mqttClient.DisconnectAsync();
    }

    private async Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs e) {
        _logger.LogDebug("{Facility} connected (ID: {id})", nameof(SendSensorMqttClient), e.ConnectResult.AssignedClientIdentifier);

        await _mqttClient.SubscribeAsync($"{_topLevelTopic}/#");
    }
}
