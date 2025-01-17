using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Entities;

public partial class MiscRev
{
    public int Id { get; set; }

    public Guid ExceptionId { get; set; }

    public int? Type { get; set; }

    public string? CardNo { get; set; }

    public string? BankCertNo { get; set; }

    public string? GlslaccountNo { get; set; }

    public string? GlslaccountName { get; set; }

    public string? Dpafno { get; set; }

    public string? CheckNo { get; set; }

    public decimal Amount { get; set; }

    public string? RefNo { get; set; }
    [JsonIgnore]
    public virtual ICollection<ExceptionItemRev> ExceptionItemRevs { get; set; } = new List<ExceptionItemRev>();
}
