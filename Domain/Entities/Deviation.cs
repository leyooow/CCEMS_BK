using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Deviation
{
    public int Id { get; set; }

    public string? Classification { get; set; }

    public string? Category { get; set; }

    public string? Deviation1 { get; set; }

    public string? RiskClassification { get; set; }
}
