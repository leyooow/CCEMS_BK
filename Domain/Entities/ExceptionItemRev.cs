using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public partial class ExceptionItemRev
{
    public string? Changes { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDateTime { get; set; }

    public string? ActionTaken { get; set; }

    public bool IsProcessed { get; set; }

    public Guid Id { get; set; }

    public string? RefNo { get; set; }

    public string? EmployeeId { get; set; } = null!;

    public string? BranchCode { get; set; } = null!;

    public string? BranchName { get; set; } = null!;

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

    public int? MonetaryRevsId { get; set; }

    public int? NonMonetaryRevsId { get; set; }

    public int? MiscRevsId { get; set; }

    public int Age { get; set; }

    public int AgingCategory { get; set; }

    public string? DeviationApprover { get; set; }

    public int DeviationCategoryId { get; set; }

    public int RiskClassificationId { get; set; }

    public int RootCause { get; set; }

    public string? Division { get; set; }

    public string? Area { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? ApprovalRemarks { get; set; }

    public string? OtherRemarks { get; set; }

    public virtual ICollection<ExceptionCodeRev> ExceptionCodeRevs { get; set; } = new List<ExceptionCodeRev>();

    public virtual MiscRev? MiscRevs { get; set; }

    public virtual MonetaryRev? MonetaryRevs { get; set; }

    public virtual NonMonetaryRev? NonMonetaryRevs { get; set; }
    [NotMapped]
    public ICollection<ActionPlan>?  ActionPlans { get; set; }
    [NotMapped]
    public bool IsCredit { get; set; }
    //public ICollection<ActionPlansDTO> ActionPlan { get; set; }
}
