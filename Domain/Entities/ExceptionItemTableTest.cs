using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class ExceptionItemTableTest
{
    public string? Id { get; set; }

    public string? RefNo { get; set; }

    public double? EmployeeId { get; set; }

    public double? BranchCode { get; set; }

    public string? BranchName { get; set; }

    public string? PersonResponsible { get; set; }

    public string? OtherPersonResponsible { get; set; }

    public double? Severity { get; set; }

    public string? DeviationApprovedBy { get; set; }

    public string? Remarks { get; set; }

    public double? RedFlag { get; set; }

    public DateTime? TransactionDate { get; set; }

    public DateTime? DateCreated { get; set; }

    public string? CreatedBy { get; set; }

    public double? Status { get; set; }

    public double? Type { get; set; }

    public double? DeviationCategoryId { get; set; }

    public double? RootCause { get; set; }

    public double? AgingCategory { get; set; }

    public string? DeviationApprover { get; set; }

    public double? Age { get; set; }

    public double? RiskClassificationId { get; set; }

    public string? Division { get; set; }

    public string? Area { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? ApprovalRemarks { get; set; }

    public string? OtherRemarks { get; set; }
}
