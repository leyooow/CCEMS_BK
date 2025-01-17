using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Entities;

public partial class BranchAccess
{
    public int Id { get; set; }

    public string? EmployeeId { get; set; }

    public int BranchId { get; set; }

    public string? UsersLoginName { get; set; }
    [JsonIgnore]
    public virtual User? UsersLoginNameNavigation { get; set; }
}
