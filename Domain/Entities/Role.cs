using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Entities;

public partial class Role
{
    public int Id { get; set; }

    public string? RoleName { get; set; }

    public string? Description { get; set; }
    [JsonIgnore]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = [];
    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = [];
}
