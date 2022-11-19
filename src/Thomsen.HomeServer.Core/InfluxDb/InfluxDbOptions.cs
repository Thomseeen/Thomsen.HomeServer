namespace Thomsen.HomeServer.Core.InfluxDb;

public record InfluxDbOptions {
    public string Url { get; set; } = default!;

    public string Token { get; set; } = default!;

    public string Org { get; set; } = default!;

    public string Bucket { get; set; } = default!;
}
