using System;
using System.Collections.Generic;

namespace Thomsen.HomeServer.Core.Tado.Models;

public record HomeModel {
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? DateTimeZone { get; init; }
    public DateTime DateCreated { get; init; }
    public string? TemperatureUnit { get; init; }
    public string? Partner { get; init; }
    public bool SimpleSmartScheduleEnabled { get; init; }
    public double AwayRadiusInMeters { get; init; }
    public bool InstallationCompleted { get; init; }
    public IncidentDetectionModel? IncidentDetection { get; init; }
    public bool AutoAssistFreeTrialEnabled { get; init; }
    public List<string>? Skills { get; init; }
    public bool ChristmasModeEnabled { get; init; }
    public bool ShowAutoAssistReminders { get; init; }
    public ContactDetailsModel? ContactDetails { get; init; }
    public AddressModel? Address { get; init; }
    public GeolocationModel? Geolocation { get; init; }
    public bool ConsentGrantSkippable { get; init; }
    public List<string>? EnabledFeatures { get; init; }
}

public record IncidentDetectionModel {
    public bool Supported { get; init; }
    public bool Enabled { get; init; }
}

public record ContactDetailsModel {
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
}

public record AddressModel {
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? ZipCode { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Country { get; init; }
}

public record GeolocationModel {
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}