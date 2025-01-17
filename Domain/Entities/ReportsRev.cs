using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class ReportsRev
{
    public string? Changes { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDateTime { get; set; }

    public string? ActionTaken { get; set; }

    public bool IsProcessed { get; set; }

    public int Id { get; set; }

    public string? FileName { get; set; }

    public string? Path { get; set; }

    public string? ActionPlan { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime DateGenerated { get; set; }

    public DateTime DateSent { get; set; }

    public DateTime ActionPlanCreated { get; set; }

    public int Status { get; set; }

    public string? BranchCodeRecipient { get; set; }

    public DateTime SendingSchedule { get; set; }

    public int ReportCoverage { get; set; }

    public int ReportCategory { get; set; }

    public DateTime CoverageDate { get; set; }

    public string? SelectedBranches { get; set; }

    public string? ToRecipients { get; set; }

    public string? Ccrecipients { get; set; }

    public Guid ReportsGuid { get; set; }

    public string? ApprovalRemarks { get; set; }
}
