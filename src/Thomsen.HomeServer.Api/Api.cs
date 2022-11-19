using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Options;

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Thomsen.HomeServer.Core.InfluxDb;

namespace Thomsen.HomeServer.Api;

internal static class Api {
    private static ApiOptions _options = default!;

    public static void ConfigureApi(this WebApplication app, ApiOptions options) {
        _options = options;

        foreach (InfluxFieldInfo field in options.InfluxFields) {
            app.MapGet($"/{field.Name}", async (HttpContext context, IInfluxDbAdapter influxDb) =>
                await GetMeasurementsAsync(context, influxDb));
        }
    }

    private static async Task<IResult> GetMeasurementsAsync(HttpContext context, IInfluxDbAdapter influx) {
        try {
            string? fieldName = ((context.GetEndpoint() as RouteEndpoint)?.RoutePattern.PathSegments[0].Parts.Single() as RoutePatternLiteralPart)?.Content;

            if (string.IsNullOrEmpty(fieldName)) {
                return Results.Problem(statusCode: (int)HttpStatusCode.NotFound);
            }

            return Results.Ok(await influx.GetAbsSensorMeasurementsAsync(
                bucket: _options.InfluxBucket,
                hourLookBack: _options.LookBackHours,
                fieldName: fieldName,
                unit: _options.InfluxFields.Single(field => field.Name == fieldName).Unit,
                groupingColumns: _options.InfluxGroupingColumns));
        } catch (Exception ex) {
            return Results.Problem(ex.Message);
        }
    }
}
