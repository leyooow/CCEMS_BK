using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class AuditLog
{
    public int Id { get; set; }

    public string? ModuleName { get; set; }

    public string? ActionType { get; set; }

    public string? ActionDesc { get; set; }

    public string? ActionBy { get; set; }

    public DateTime DateEntry { get; set; }
}
