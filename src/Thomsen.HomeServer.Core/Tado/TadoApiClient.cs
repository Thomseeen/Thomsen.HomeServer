using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Thomsen.HomeServer.Core.Mqtt.Models;
using Thomsen.HomeServer.Core.Tado.Models;

namespace Thomsen.HomeServer.Core.Tado;

public class TadoApiClient : ITadoApiClient {
    private readonly ILogger _logger;

    private readonly IOptions<TadoOptions> _options;

    private readonly IHttpClientFactory _httpClientFactory;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private ApiEnvModel? _env;
    private BearerTokenModel? _token;
    private UserModel? _user;

    public TadoApiClient(ILogger<TadoApiClient> logger, IOptions<TadoOptions> options, IHttpClientFactory httpClientFactory) {
        _logger = logger;
        _options = options;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<TadoSensorModel[]> GetSensorModelsAsync() {
        HomeModel home = await GetHomeAsync();
        ZoneModel[] zones = await GetZonesAsync();

        Task<TadoSensorModel>[] sensorTasks = zones.Select(async zone => {
            ZoneStateModel state = await GetZoneStateAsync(zone.Id);

            return new TadoSensorModel() {
                ZoneId = zone.Id,
                ZoneName = zone.Name ?? default!,
                HomeName = home.Name ?? default!,
                Time = state.SensorDataPoints?.InsideTemperature?.Timestamp ?? default,
                Temperature = state.SensorDataPoints?.InsideTemperature?.Celsius ?? default,
                Humidity = state.SensorDataPoints?.Humidity?.Percentage ?? default,
                TargetTemperatue = state.Setting?.Temperature?.Celsius ?? 0
            };
        }).ToArray();

        return await Task.WhenAll(sensorTasks);
    }

    private async Task<ZoneStateModel> GetZoneStateAsync(int id) {
        return await GetHomeDataAsync<ZoneStateModel>($"zones/{id}/state");
    }

    private async Task<ZoneModel[]> GetZonesAsync() {
        return await GetHomeDataAsync<ZoneModel[]>("zones");
    }

    private async Task<HomeModel> GetHomeAsync() {
        return await GetHomeDataAsync<HomeModel>();
    }

    private async Task<T> GetHomeDataAsync<T>(string endPoint = "") {
        _user ??= await GetApiCallAsync<UserModel>("me");

        return await GetApiCallAsync<T>($"homes/{_user.Homes.First().Id}/{endPoint}");
    }

    private async Task<T> GetApiCallAsync<T>(string endPoint) {
        _env ??= await GetEnvDataAsync();

        HttpClient client = _httpClientFactory.CreateClient(nameof(TadoApiClient));
        using HttpRequestMessage req = await BuildAuthorizedGetRequestAsync($"{_env.ApiEndpointV2}/{endPoint}");
        using HttpResponseMessage res = await client.SendAsync(req);

        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions)
            ?? throw new InvalidOperationException($"Api call result was null");
    }

    private async Task<BearerTokenModel> GetBearerTokenAsync() {
        _env ??= await GetEnvDataAsync();

        if (_token is not null && !_token.IsExpired) {
            return _token;
        }

        string formData = _token is not null && _token.IsExpired
            ? GetRefreshBearerTokenFormData(_env, _token)
            : GetBearerTokenFormData(_env, _options.Value.User, _options.Value.Password);

        HttpClient client = _httpClientFactory.CreateClient(nameof(TadoApiClient));
        using HttpRequestMessage req = BuildPostFormDataRequest(uri: $"{_env.OAuthApiEndpoint}/token", formData: formData);
        using HttpResponseMessage res = await client.SendAsync(req);

        res.EnsureSuccessStatusCode();

        _logger.LogDebug("Tado Api got new/refreshed token");

        return await res.Content.ReadFromJsonAsync<BearerTokenModel>(_jsonSerializerOptions)
            ?? throw new InvalidOperationException($"Api call result was null");
    }

    private async Task<ApiEnvModel> GetEnvDataAsync() {
        HttpClient client = _httpClientFactory.CreateClient(nameof(TadoApiClient));
        using HttpRequestMessage req = new(HttpMethod.Get, _options.Value.EnvDataUrl);
        using HttpResponseMessage res = await client.SendAsync(req);

        res.EnsureSuccessStatusCode();

        return ApiEnvModel.Parse(await res.Content.ReadAsStringAsync());
    }

    private async Task<HttpRequestMessage> BuildAuthorizedGetRequestAsync(string uri) {
        _token = await GetBearerTokenAsync();

        return new HttpRequestMessage(HttpMethod.Get, uri) {
            Headers = {
                Authorization = new AuthenticationHeaderValue(_token.TokenType, _token.AccessToken)
            }
        };
    }

    private static HttpRequestMessage BuildPostFormDataRequest(string uri, string formData) {
        return new HttpRequestMessage(HttpMethod.Post, uri) {
            Content = new StringContent(formData, Encoding.ASCII, "application/x-www-form-urlencoded")
        };
    }

    private static string GetBearerTokenFormData(ApiEnvModel env, string user, string password) {
        return $"client_id={env.OAuthClientId}&grant_type=password&scope=home.user&username={user}&password={password}&client_secret={env.OAuthClientSecret}";
    }

    private static string GetRefreshBearerTokenFormData(ApiEnvModel env, BearerTokenModel token) {
        return $"client_id={env.OAuthClientId}&grant_type=refresh_token&scope=home.user&refresh_token={token.RefreshToken}&client_secret={env.OAuthClientSecret}";
    }
}
