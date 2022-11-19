using System;
using System.Collections.Generic;

namespace Thomsen.HomeServer.Core.Tado.Models;

public record ZoneModel {
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? Type { get; init; }
    public DateTime DateCreated { get; init; }
    public List<string>? DeviceTypes { get; init; }
    public List<Device>? Devices { get; init; }
    public bool ReportAvailable { get; init; }
    public bool ShowScheduleinitup { get; init; }
    public bool SupportsDazzle { get; init; }
    public bool DazzleEnabled { get; init; }
    public DazzleMode? DazzleMode { get; init; }
    public OpenWindowDetection? OpenWindowDetection { get; init; }
}

public record Device {
    public string? DeviceType { get; init; }
    public string? SerialNo { get; init; }
    public string? ShortSerialNo { get; init; }
    public string? CurrentFwVersion { get; init; }
    public ConnectionState? ConnectionState { get; init; }
    public Characteristics? Characteristics { get; init; }
    public MountingState? MountingState { get; init; }
    public string? BatteryState { get; init; }
    public string? Orientation { get; init; }
    public bool ChildLockEnabled { get; init; }
    public List<string>? Duties { get; init; }
}

public record DazzleMode {
    public bool Supported { get; init; }
    public bool Enabled { get; init; }
}

public record OpenWindowDetection {
    public bool Supported { get; init; }
    public bool Enabled { get; init; }
    public int TimeoutInSeconds { get; init; }
}

public record ConnectionState {
    public bool Value { get; init; }
    public DateTime Timestamp { get; init; }
}

public record Characteristics {
    public List<string>? Capabilities { get; init; }
}

public record MountingState {
    public string? Value { get; init; }
    public DateTime Timestamp { get; init; }
}
