using System.Collections.Generic;
using System.Threading.Tasks;

using Thomsen.HomeServer.Core.InfluxDb.Models;

namespace Thomsen.HomeServer.Core.InfluxDb {
    public interface IInfluxDbAdapter {
        Task<SensorMeasurements[]> GetAbsSensorMeasurementsAsync(string bucket, int hourLookBack, string fieldName, string unit, params string[] groupingColumns);
        Task<Dictionary<Identifier, Measurement>> GetSingleValueAsync(string bucket, int hourLookBack, string fieldName, string unit, SingleValueAggregation aggregation, params string[] groupingColumns);
    }
}