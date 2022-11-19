namespace Thomsen.HomeServer.Core.Tado.Models {
    internal record ApiEnvModel {
        public string ApiEndpoint { get; init; } = default!;

        public string ApiEndpointV2 { get; init; } = default!;

        public string OAuthApiEndpoint { get; init; } = default!;

        public string OAuthClientId { get; init; } = default!;

        public string OAuthClientSecret { get; init; } = default!;

        public static ApiEnvModel Parse(string rawEnvData) {
            return new ApiEnvModel {
                ApiEndpoint = GetValueFromJSScript("tgaRestApiEndpoint", rawEnvData),
                ApiEndpointV2 = GetValueFromJSScript("tgaRestApiV2Endpoint", rawEnvData),
                OAuthApiEndpoint = GetValueFromJSScript("apiEndpoint", rawEnvData),
                OAuthClientId = GetValueFromJSScript("clientId", rawEnvData),
                OAuthClientSecret = GetValueFromJSScript("clientSecret", rawEnvData)
            };
        }

        private static string GetValueFromJSScript(string key, string script) {
            int keyIdx = script.IndexOf($"{key}:");
            int startIdx = script.IndexOf('\'', keyIdx);
            int endIdx = script.IndexOf('\'', startIdx + 1);

            return script.Substring(startIdx + 1, endIdx - startIdx - 1);
        }
    }
}
