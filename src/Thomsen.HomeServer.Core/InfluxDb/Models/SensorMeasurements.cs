namespace Thomsen.HomeServer.Core.InfluxDb.Models;

public record SensorMeasurements {
    public Identifier Identifier { get; init; } = default!;

    public Measurement Last { get; init; } = default!;

    public Measurement Min { get; init; } = default!;

    public Measurement Max { get; init; } = default!;
}
