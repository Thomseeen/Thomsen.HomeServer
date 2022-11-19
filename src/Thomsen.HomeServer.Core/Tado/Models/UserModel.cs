using System.Collections.Generic;

namespace Thomsen.HomeServer.Core.Tado.Models;

public record UserModel {
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Username { get; init; }
    public string? Id { get; init; }
    public List<Home> Homes { get; init; } = default!;
    public string? Locale { get; init; }
    public List<MobileDevice> MobileDevices { get; init; } = default!;
}

public record Home {
    public int Id { get; init; }
    public string? Name { get; init; }
}

public record MobileDevice {
    public string? Name { get; init; }
    public int Id { get; init; }
    public Settings? Settings { get; init; }
    public DeviceMetadata? DeviceMetadata { get; init; }
}

public record Settings {
    public bool GeoTrackingEnabled { get; init; }
    public bool OnDemandLogRetrievalEnabled { get; init; }
    public PushNotifications? PushNotifications { get; init; }
}

public record DeviceMetadata {
    public string? Platform { get; init; }
    public string? OsVersion { get; init; }
    public string? Model { get; init; }
    public string? Locale { get; init; }
}

public record PushNotifications {
    public bool LowBatteryReminder { get; init; }
    public bool AwayModeReminder { get; init; }
    public bool HomeModeReminder { get; init; }
    public bool OpenWindowReminder { get; init; }
    public bool EnergySavingsReportReminder { get; init; }
    public bool IncidentDetection { get; init; }
}
