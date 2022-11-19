using System;

namespace Thomsen.HomeServer.Core.InfluxDb.Models;

public record Measurement {
    public DateTime TimeStamp { get; init; }

    public double Value { get; init; }

    public string Unit { get; init; } = default!;
}