using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Group
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateModified { get; set; }

    public string? CreatedBy { get; set; }

    public string? Area { get; set; }

    public string? Division { get; set; }
}
