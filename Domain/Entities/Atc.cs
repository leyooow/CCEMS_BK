using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Atc
{
    public Guid AtcCode { get; set; }

    public decimal AtcTax { get; set; }

    public string? Description { get; set; }

    public Guid Corporate { get; set; }

    public string? TaxCode { get; set; }

    public bool IsDeleted { get; set; }
}
