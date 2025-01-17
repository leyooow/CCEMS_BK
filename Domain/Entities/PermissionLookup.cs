using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class PermissionLookup
{
    public int Id { get; set; }

    public string? Module { get; set; }

    public string? Function { get; set; }
}
