using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Entities;

public partial class RolePermission
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string? Permission { get; set; }
    [JsonIgnore]
    public virtual Role Role { get; set; } = null!;
}
