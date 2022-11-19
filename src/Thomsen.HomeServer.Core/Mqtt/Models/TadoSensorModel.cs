using InfluxDB.Client.Writes;

using System;
using System.Text.Json.Serialization;

namespace Thomsen.HomeServer.Core.Mqtt.Models;

public record TadoSensorModel : ISensorModel {
    [JsonPropertyName("ZoneId")]
    public int ZoneId { get; init; }

    [JsonPropertyName("ZoneName")]
    public string ZoneName { get; init; } = default!;

    [JsonPropertyName("HomeName")]
    public string HomeName { get; init; } = default!;

    [JsonPropertyName("TargetTemperatue")]
    public double TargetTemperatue { get; init; }

    [JsonPropertyName("Time")]
    public DateTime Time { get; init; }

    [JsonPropertyName("Temperature")]
    public double Temperature { get; init; }

    [JsonPropertyName("Humidity")]
    public double Humidity { get; init; }

    [JsonIgnore]
    public static string TopicIdPart => "tado";

    [JsonIgnore]
    public string TopicAdditionalParts => $"zones/{ZoneName}";

    public PointData ToPointData() {
        return PointData.Measurement("temperature")
            .Tag("Type", GetType().Name)
            .Timestamp(Time.ToUniversalTime(), InfluxDB.Client.Api.Domain.WritePrecision.Ns)
            .Tag(nameof(ZoneId), ZoneId.ToString())
            .Tag(nameof(ZoneName), ZoneName)
            .Tag(nameof(HomeName), HomeName)
            .Field(nameof(Temperature), Temperature)
            .Field(nameof(Humidity), Humidity)
            .Field(nameof(TargetTemperatue), TargetTemperatue);
    }
}
