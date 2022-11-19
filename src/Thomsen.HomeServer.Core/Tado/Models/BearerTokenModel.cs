using System;
using System.Text.Json.Serialization;

namespace Thomsen.HomeServer.Core.Tado.Models;

internal record BearerTokenModel {
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = default!;

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = default!;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = default!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("scope")]
    public string Scope { get; init; } = default!;

    [JsonPropertyName("jti")]
    public string Jti { get; init; } = default!;

    [JsonIgnore]
    public DateTime CreationTime { get; init; } = DateTime.UtcNow;

    [JsonIgnore]
    public bool IsExpired => DateTime.UtcNow >= CreationTime.AddSeconds(ExpiresIn);
}