using InfluxDB.Client.Writes;

using System;
using System.Text.Json.Serialization;

namespace Thomsen.HomeServer.Core.Mqtt.Models;

public record RtlSensorModel : ISensorModel {
    [JsonPropertyName("protocol")]
    public int Protocol { get; init; }

    [JsonPropertyName("model")]
    public string Model { get; init; } = default!;

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("channel")]
    public int Channel { get; init; }

    [JsonPropertyName("battery_ok")]
    public float BatteryOk { get; init; }

    [JsonPropertyName("flags")]
    public int Flags { get; init; }

    [JsonPropertyName("mic")]
    public string Mic { get; init; } = default!;

    [JsonPropertyName("time")]
    public DateTime Time { get; init; }

    [JsonPropertyName("temperature_C")]
    public double Temperature { get; init; }

    [JsonPropertyName("humidity")]
    public double Humidity { get; init; }

    [JsonIgnore]
    public static string TopicIdPart => "rtl_433";

    [JsonIgnore]
    public string TopicAdditionalParts => $"{Protocol}/{Channel}";

    public PointData ToPointData() {
        return PointData.Measurement("temperature")
            .Tag("Type", GetType().Name)
            .Timestamp(Time.ToUniversalTime(), InfluxDB.Client.Api.Domain.WritePrecision.Ns)
            .Tag(nameof(Protocol), Protocol.ToString())
            .Tag(nameof(Model), Model)
            .Tag(nameof(Id), Id.ToString())
            .Tag(nameof(Channel), Channel.ToString())
            .Field(nameof(Temperature), Temperature)
            .Field(nameof(Humidity), Humidity);
    }
}

