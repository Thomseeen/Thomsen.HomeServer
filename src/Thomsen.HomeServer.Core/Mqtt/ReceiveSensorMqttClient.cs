using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using MQTTnet.Client;

using System;
using System.Threading;
using System.Threading.Tasks;

using Thomsen.HomeServer.Core.Mqtt.Models;

namespace Thomsen.HomeServer.Core.Mqtt;

public class ReceiveSensorMqttClient : IReceiveSensorMqttClient {
    private static readonly TimeSpan RECONNECT_INTERVAL = TimeSpan.FromSeconds(5);

    private readonly ILogger _logger;

    private readonly string _topLevelTopic;

    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttClientOptions;

    private readonly MqttSensorMapper _mqttSensorMapper;

    public event ReceivedSensorHandler? ReceivedSensor;

    public delegate void ReceivedSensorHandler(object sender, ISensorModel sensor);

    private void RaiseReceivedSensorHandler(ISensorModel sensor) {
        ReceivedSensor?.Invoke(this, sensor);
    }

    public ReceiveSensorMqttClient(ILogger<ReceiveSensorMqttClient> logger, IConfiguration configuration, IMqttClient mqttClient, MqttClientOptions mqttClientOptions, MqttSensorMapper mqttSensorMapper) {
        _logger = logger;
        _topLevelTopic = configuration.GetValue<string>("General:MqttTopLevelTopic");
        _mqttClient = mqttClient;
        _mqttClientOptions = mqttClientOptions;
        _mqttSensorMapper = mqttSensorMapper;

        _mqttClientOptions.ClientId = $"{nameof(ReceiveSensorMqttClient)}-{Guid.NewGuid()}";
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogDebug("{Facility} starting", nameof(ReceiveSensorMqttClient));

        _mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
        _mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
        _mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;

        await _mqttClient.ConnectAsync(_mqttClientOptions, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        _logger.LogDebug("{Facility} stopping", nameof(ReceiveSensorMqttClient));

        _mqttClient.ConnectedAsync -= MqttClient_ConnectedAsync;
        _mqttClient.DisconnectedAsync -= MqttClient_DisconnectedAsync;
        _mqttClient.ApplicationMessageReceivedAsync -= MqttClient_ApplicationMessageReceivedAsync;

        if (_mqttClient.IsConnected) {
            await _mqttClient.UnsubscribeAsync($"{_topLevelTopic}/#", cancellationToken);
            await _mqttClient.DisconnectAsync();
        }
    }

    private async Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs e) {
        _logger.LogDebug("{Facility} connected (ID: {id})", nameof(ReceiveSensorMqttClient), e.ConnectResult.AssignedClientIdentifier);

        await _mqttClient.SubscribeAsync($"{_topLevelTopic}/#");
    }

    private async Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs e) {
        _logger.LogWarning("{Facility} disconnected (Reason: {reason}), retrying in {interval}", nameof(ReceiveSensorMqttClient), e.Reason, RECONNECT_INTERVAL);

        while (!_mqttClient.IsConnected) {
            try {
                await Task.Delay(RECONNECT_INTERVAL);
                await _mqttClient.ConnectAsync(_mqttClientOptions);
            } catch {
                _logger.LogWarning("{Facility} reconnecting failed, retrying in {interval}", nameof(ReceiveSensorMqttClient), RECONNECT_INTERVAL);
            }
        }
    }

    private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e) {
        _logger.LogTrace("{Facility} received message on topic: {topic}", nameof(ReceiveSensorMqttClient), e.ApplicationMessage.Topic);

        if (_mqttSensorMapper.TryParseMqttMsg(e.ApplicationMessage, out ISensorModel? sensor) && sensor is not null) {
            RaiseReceivedSensorHandler(sensor);
        }

        return Task.CompletedTask;
    }
}
