using Microsoft.Extensions.Configuration;

using MQTTnet;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

using Thomsen.HomeServer.Core.Mqtt.Models;

namespace Thomsen.HomeServer.Core.Mqtt;

public class MqttSensorMapper {
    private readonly Dictionary<string, Type> _models = new();

    private readonly string _topLevelTopic;

    public MqttSensorMapper(IConfiguration configuration) {
        _topLevelTopic = configuration.GetValue<string>("General:MqttTopLevelTopic");

        IEnumerable<Type> sensorModelTypes = Assembly
            .GetAssembly(typeof(ISensorModel))!
            .GetTypes()
            .Where(type => type.IsAssignableTo(typeof(ISensorModel)) && type.IsClass);

        foreach (Type type in sensorModelTypes) {
            _models.Add(
                key: type.GetProperty(nameof(ISensorModel.TopicIdPart))?.GetValue(null)?.ToString()
                    ?? throw new InvalidOperationException(),
                value: type);
        }
    }

    public MqttApplicationMessage ToMqttMsg<T>(T sensor) where T : ISensorModel {
        return new MqttApplicationMessageBuilder()
            .WithTopic($"{_topLevelTopic}/{T.TopicIdPart.Trim('/')}/{sensor.TopicAdditionalParts.Trim('/')}")
            .WithPayload(Serialize(sensor))
            .Build();
    }

    public bool TryParseMqttMsg(MqttApplicationMessage mqttMsg, out ISensorModel? sensor) {
        sensor = null;

        string[] topicParts = mqttMsg.Topic.Split('/');

        if (topicParts[0] != _topLevelTopic) {
            // No sensor message
            return false;
        }

        try {
            Type modelType = _models.Single(model => model.Key == topicParts[1]).Value;
            sensor = Deserialize(modelType, mqttMsg.Payload);
            return true;
        } catch {
            // Error
            return false;
        }
    }

    private static string Serialize<T>(T obj) {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private static ISensorModel? Deserialize(Type type, byte[] data) {
        Utf8JsonReader reader = new(data);
        return JsonSerializer.Deserialize(ref reader, type, new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }) as ISensorModel;
    }
}
