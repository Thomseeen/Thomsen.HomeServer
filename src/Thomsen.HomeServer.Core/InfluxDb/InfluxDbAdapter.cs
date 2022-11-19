using InfluxDB.Client;
using InfluxDB.Client.Core.Flux.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Thomsen.HomeServer.Core.InfluxDb.Models;

namespace Thomsen.HomeServer.Core.InfluxDb;

public enum SingleValueAggregation {
    First,
    Last,
    Min,
    Max
}

public class InfluxDbAdapter : IInfluxDbAdapter {
    private readonly IInfluxDBClient _influxClient;

    public InfluxDbAdapter(IInfluxDBClient influxDbClient) {
        _influxClient = influxDbClient;
    }

    public async Task<SensorMeasurements[]> GetAbsSensorMeasurementsAsync(string bucket, int hourLookBack, string fieldName, string unit, params string[] groupingColumns) {
        Dictionary<Identifier, Measurement> last = await GetSingleValueAsync(bucket, hourLookBack, fieldName, unit, SingleValueAggregation.Last, groupingColumns);
        Dictionary<Identifier, Measurement> min = await GetSingleValueAsync(bucket, hourLookBack, fieldName, unit, SingleValueAggregation.Min, groupingColumns);
        Dictionary<Identifier, Measurement> max = await GetSingleValueAsync(bucket, hourLookBack, fieldName, unit, SingleValueAggregation.Max, groupingColumns);

        return last.Keys.Select(key => new SensorMeasurements() {
            Identifier = key,
            Last = last[key],
            Min = min[key],
            Max = max[key]
        }).ToArray();
    }

    public async Task<Dictionary<Identifier, Measurement>> GetSingleValueAsync(string bucket, int hourLookBack, string fieldName, string unit, SingleValueAggregation aggregation, params string[] groupingColumns) {
        string query =
            $"from(bucket:\"{bucket}\")" +
            $"   |> range(start: -{hourLookBack}h)" +
            $"   |> filter(fn: (r) => r[\"_field\"] == \"{fieldName}\")" +
            $"   |> group(columns: [{string.Join(",", groupingColumns.Select(col => $"\"{col}\""))}])" +
            $"   |> {GetAggregationFunction(aggregation)}";

        List<FluxTable> tables = await _influxClient.GetQueryApi().QueryAsync(query);

        return tables.ToDictionary(table => {
            FluxRecord record = table.Records.Single();
            KeyValuePair<string, object> name = record.Values.Single(val => groupingColumns.Contains(val.Key));
            return new Identifier() { Key = name.Key, Value = name.Value.ToString() };
        }, table => {
            FluxRecord record = table.Records.Single();
            return new Measurement() {
                TimeStamp = record.GetTimeInDateTime()?.ToLocalTime() ?? throw new InvalidCastException(),
                Unit = unit,
                Value = (double)record.GetValue()
            };
        });
    }

    private static string GetAggregationFunction(SingleValueAggregation aggregation) {
        return aggregation switch {
            SingleValueAggregation.First => "first()",
            SingleValueAggregation.Last => "last()",
            SingleValueAggregation.Min => "min()",
            SingleValueAggregation.Max => "max()",
            _ => throw new InvalidOperationException()
        };
    }
}
