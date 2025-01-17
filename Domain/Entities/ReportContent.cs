using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class ReportContent
{
    public Guid Id { get; set; }

    public int ReportId { get; set; }

    public string? ExceptionNo { get; set; }

    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }

    public string? Area { get; set; }

    public string? Division { get; set; }

    public string? TransactionDate { get; set; }

    public string? Aging { get; set; }

    public string? AgingCategory { get; set; }

    public string? Process { get; set; }

    public string? AccountNo { get; set; }

    public string? AccountName { get; set; }

    public string? Deviation { get; set; }

    public string? RiskClassification { get; set; }

    public string? DeviationCategory { get; set; }

    public decimal? Amount { get; set; }

    public string? PersonResponsible { get; set; }

    public string? OtherPersonResponsible { get; set; }

    public string? Remarks { get; set; }

    public string? ActionPlan { get; set; }

    public string? EncodedBy { get; set; }

    public string? RootCause { get; set; }

    public string? DeviationApprover { get; set; }

    public virtual Report Report { get; set; } = null!;
}
