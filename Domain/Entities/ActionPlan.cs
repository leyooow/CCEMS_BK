using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class ActionPlan
{
    public Guid Id { get; set; }

    public string? ExceptionItemRefNo { get; set; }

    public int? ReportId { get; set; }

    public string? CreatedBy { get; set; }

    public string? ActionPlan1 { get; set; }

    public DateTime DateCreated { get; set; }

    public string? Type { get; set; }

    public Guid? ExceptionItemRevsId { get; set; }
}
