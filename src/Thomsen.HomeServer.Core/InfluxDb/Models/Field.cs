using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thomsen.HomeServer.Core.InfluxDb.Models;

public record Identifier {
    public string Key { get; set; } = default!;

    public string? Value { get; set; } = default!;
}
