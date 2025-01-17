using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Entities;

public partial class Monetary
{
    public int Id { get; set; }

    public string SequenceNo { get; set; } = null!;

    public string BdstellerId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? TransCode { get; set; }

    public string? TransDescription { get; set; }

    public string? CreditAccountNo { get; set; }

    public string? CreditAccountName { get; set; }

    public string? DebitAccountNo { get; set; }

    public string? DebitAccountName { get; set; }

    public Guid ExceptionId { get; set; }

    public string? RefNo { get; set; }

    public int? Currency { get; set; }
    [JsonIgnore]
    public virtual ExceptionItem? RefNoNavigation { get; set; }
}
