namespace Thomsen.HomeServer.Core.Mqtt;

public record MqttOptions {
    public string Address { get; set; } = default!;

    public int Port { get; set; } = 8883;

    public bool UseTls { get; set; } = true;

    public string User { get; set; } = default!;

    public string Password { get; set; } = default!;
}
