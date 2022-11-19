using System;

namespace Thomsen.HomeServer.Core.Tado.Models;

public record ZoneStateModel {
    public string? TadoMode { get; init; }
    public bool GeolocationOverride { get; init; }
    public DateTime? GeolocationOverrideDisableTime { get; init; }
    public object? Preparation { get; init; } // #TODO: Unknown
    public Setting? Setting { get; init; }
    public object? OverlayType { get; init; }// #TODO: Unknown
    public object? Overlay { get; init; }// #TODO: Unknown
    public object? OpenWindow { get; init; }// #TODO: Unknown
    public NextScheduleChange? NextScheduleChange { get; init; }
    public NextTimeBlock? NextTimeBlock { get; init; }
    public Link? Link { get; init; }
    public ActivityDataPoints? ActivityDataPoints { get; init; }
    public SensorDataPoints? SensorDataPoints { get; init; }
}

public record Setting {
    public string? Type { get; init; }
    public string? Power { get; init; }
    public Temperature? Temperature { get; init; }
}

public record NextScheduleChange {
    public DateTime Start { get; init; }
    public Setting? Setting { get; init; }
}

public record NextTimeBlock {
    public DateTime Start { get; init; }
}

public record Link {
    public string? State { get; init; }
}

public record ActivityDataPoints {
    public HeatingPower? HeatingPower { get; init; }
}

public record SensorDataPoints {
    public InsideTemperature? InsideTemperature { get; init; }
    public Humidity? Humidity { get; init; }
}

public record Temperature {
    public double Celsius { get; init; }
    public double Fahrenheit { get; init; }
}

public record HeatingPower {
    public string? Type { get; init; }
    public double Percentage { get; init; }
    public DateTime Timestamp { get; init; }
}

public record InsideTemperature {
    public double Celsius { get; init; }
    public double Fahrenheit { get; init; }
    public DateTime Timestamp { get; init; }
    public string? Type { get; init; }
    public Precision? Precision { get; init; }
}

public record Humidity {
    public string? Type { get; init; }
    public double Percentage { get; init; }
    public DateTime Timestamp { get; init; }
}

public record Precision {
    public double Celsius { get; init; }
    public double Fahrenheit { get; init; }
}
