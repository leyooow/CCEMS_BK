using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class SubRefNoSequence
{
    public int Id { get; set; }

    public string? Ern { get; set; }

    public string? Series { get; set; }

    public DateTime Date { get; set; }
}
