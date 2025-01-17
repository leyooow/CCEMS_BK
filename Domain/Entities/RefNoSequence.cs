using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class RefNoSequence
{
    public int Id { get; set; }

    public int Series { get; set; }

    public DateTime Date { get; set; }
}
