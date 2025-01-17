using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class ExceptionItemTest
{
    public Guid Id { get; set; }

    public string RefNo { get; set; } = null!;

    public string EmployeeId { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string BranchName { get; set; } = null!;

    public string? PersonResponsible { get; set; }

    public string? OtherPersonResponsible { get; set; }

    public int Severity { get; set; }

    public string? DeviationApprovedBy { get; set; }

    public string? Remarks { get; set; }

    public bool RedFlag { get; set; }

    public DateTime TransactionDate { get; set; }

    public DateTime DateCreated { get; set; }

    public string? CreatedBy { get; set; }

    public int Status { get; set; }

    public int Type { get; set; }

    public int DeviationCategoryId { get; set; }

    public int RootCause { get; set; }

    public int AgingCategory { get; set; }

    public string? DeviationApprover { get; set; }

    public int Age { get; set; }

    public int RiskClassificationId { get; set; }

    public string? Division { get; set; }

    public string? Area { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? ApprovalRemarks { get; set; }

    public string? OtherRemarks { get; set; }
}
