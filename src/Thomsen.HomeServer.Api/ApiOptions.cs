using System;

namespace Thomsen.HomeServer.Api;

internal record ApiOptions {
    public int LookBackHours { get; set; }

    public string InfluxBucket { get; set; } = default!;

    public InfluxFieldInfo[] InfluxFields { get; set; } = Array.Empty<InfluxFieldInfo>();

    public string[] InfluxGroupingColumns { get; set; } = Array.Empty<string>();
}

public record InfluxFieldInfo {
    public string Name { get; init; } = default!;

    public string Unit { get; init; } = default!;
}
