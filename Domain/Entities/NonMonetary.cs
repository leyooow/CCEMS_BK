using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Entities;

public partial class NonMonetary
{
    public int Id { get; set; }

    public int? Category { get; set; }

    public string Cifnumber { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string? CustomerAccountNo { get; set; }

    public Guid ExceptionId { get; set; }

    public string? RefNo { get; set; }
    [JsonIgnore]
    public virtual ExceptionItem? RefNoNavigation { get; set; }
}
