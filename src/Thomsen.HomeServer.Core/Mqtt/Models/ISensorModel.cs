using InfluxDB.Client.Writes;

using System;

namespace Thomsen.HomeServer.Core.Mqtt.Models;

public interface ISensorModel {
    public DateTime Time { get; init; }

    public double Temperature { get; init; }

    public double Humidity { get; init; }

    public static abstract string TopicIdPart { get; }

    public string TopicAdditionalParts { get; }

    public PointData ToPointData();
}