using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class DeviationCategoryLookup
{
    public int Id { get; set; }

    public int DeviationCategory { get; set; }

    public string? Description { get; set; }
}
