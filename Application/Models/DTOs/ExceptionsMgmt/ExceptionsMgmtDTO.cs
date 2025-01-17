using Application.Models.DTOs.Miscellaneous;
using Application.Models.DTOs.Monetary;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models.DTOs.ExceptionsMgmt
{
    public class ExceptionItemDTO
    {
        public Guid Id { get; set; }

        [Key]
        public string RefNo { get; set; }

        [Required]
        [RegularExpression("([+-]?[0-9]+(?:\\.[0-9]*)?)", ErrorMessage = "Only allows numeric values.")]
        [StringLength(10, ErrorMessage = "Max length is 10.")]
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [Required]
        [StringLength(3)]
        [Display(Name = "Branch Code")]
        public string BranchCode { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [StringLength(100)]
        [Display(Name = "Employee Responsible")]
        public string PersonResponsible { get; set; }

        [StringLength(200)]
        [Display(Name = "Other Employee Responsible")]
        public string OtherPersonResponsible { get; set; }

        [Required]
        public SeverityDTO Severity { get; set; }

        [StringLength(50)]
        [Display(Name = "Deviation Approved By")]
        public string DeviationApprovedBy { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }

        [Display(Name = "Red Flag")]
        public bool RedFlag { get; set; }

        [Required]
        //[FutureDatedValidation]
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }

        [Required]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [StringLength(25)]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        public MainStatusDTO Status { get; set; }

        [Display(Name = "Transaction Type")]
        public TransactionTypeEnumDTO Type { get; set; }

        public ICollection<ExceptionCode> ExceptionCodes { get; set; }

        public MonetaryDTO Monetary { get; set; }

        public NonMonetary NonMonetary { get; set; }

        public MiscellaneousDTO Misc { get; set; }

        public ICollection<ActionPlansDTO> ActionPlan { get; set; }



        //New
        public int DeviationCategoryId { get; set; }

        public RootCauseDTO RootCause { get; set; }

        public AgingCategoryDTO AgingCategory { get; set; }

        public string DeviationApprover { get; set; }

        public int Age { get; set; } //calculated by Days

        public int RiskClassificationId { get; set; }

        [StringLength(50)]
        [Display(Name = "Division")]
        public string Division { get; set; }
        [StringLength(50)]
        [Display(Name = "Area")]
        public string Area { get; set; }

        public DateTime EntryDate { get; set; }

        public string ApprovalRemarks { get; set; }

        public string OtherRemarks { get; set; }

    }

    public enum SeverityDTO
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum MainStatusDTO
    {
        Default = -1,
        [Display(Name = "For Approval")]
        PendingApproval = 0,
        Open = 1,
        Closed = 2,
    }

    public enum TransactionTypeEnumDTO
    {
        Monetary = 1,
        [Display(Name = "Non Monetary")]
        NonMonetary = 2,
        Miscellaneous = 3
    }

    public enum RootCauseDTO
    {
        [Display(Name = "Employee Lapse")]
        EmployeeLapses = 1,
        [Display(Name = "Business Decision")]
        BusinessDecision = 2
    }

    public enum AgingCategoryDTO
    {
        [Display(Name = "≤ 7D banking days")]
        LessEqual7Days,
        [Display(Name = "≤ 15D banking days")]
        LessEqual15Days,
        [Display(Name = "≤ 30D banking days")]
        LessEqual30Days,
        [Display(Name = "≤ 45D banking days")]
        LessEqual45Days,
        [Display(Name = "≤ 180D banking days")]
        LessEqual180Days,
        [Display(Name = "≤ 1Y (251 banking days)")]
        LessEqual1Year,
        [Display(Name = "≤ 2Y (2 x 251 banking days)")]
        LessEqual2Year,
        [Display(Name = "≤ 3Y (3 x 251 banking days)")]
        LessEqual3Year,
        [Display(Name = "≤ 4Y (4 x 251 banking days)")]
        LessEqual4Year,
        [Display(Name = "≤ 5Y (5 x 251 banking days)")]
        LessEqual5Year
    }

    public enum CurrencyDTO
    {
        PHP = 1,
        USD = 2,
        EUR = 3,
        YEN = 4
    }

    public class ExceptionViewDTO
    {
        [Display(Name = "Employee ID")]
        public string? EmployeeID { get; set; }

        [Display(Name = "Employee Name")]
        public string? EmployeeName { get; set; }
        public ExceptionItem? ExceptionItem { get; set; }
        public ExceptionItemRev? ExceptionItemRevs { get; set; }

        //[ValidateEmptyDeviationList]
        public List<int>? SelectedExCodes { get; set; }

        public ICollection<ActionPlansDTO>? ActionPlans { get; set; }

        public ActionPlansDTO? ActionPlan { get; set; }
        public List<SubExceptionsListViewDTO>? SubExceptionItems { get; set; }

        public bool HasPendingUpdate { get; set; }

        public bool HasFormChanges { get; set; }

        public string? Request { get; set; }

        public string? ApprovalRemarks { get; set; }
    }

    public class SubExceptionsListViewDTO
    {
        public int Id { get; set; }
        public string SubReferenceNo { get; set; }
        public int ExCode { get; set; }
        [JsonIgnore]
        public ExceptionItem? ExItem { get; set; }
        public string ExItemRefNo { get; set; }
        public DeviationStatusDTO DeviationStatus { get; set; }
        public ApprovalStatusDTO ApprovalStatus { get; set; }
        public DateTime DateCreated { get; set; }
        public string ExCodeDescription { get; set; }
        public string DeviationCategory { get; set; }
        public string RiskClassification { get; set; }
        public string? Request { get; set; }
    }
}
