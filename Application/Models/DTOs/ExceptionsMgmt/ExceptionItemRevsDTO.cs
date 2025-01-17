using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.DTOs.Revisions;
using Application.Models.DTOs.Monetary;
using Application.Models.DTOs.Miscellaneous;

namespace Application.Models.DTOs.ExceptionsMgmt
{
    public class ExceptionItemRevsDTO : RevisionsDTO
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

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

        public ICollection<ExceptionCodeRevsDTO> ExCodeRevs { get; set; }

        public MonetaryRevsDTO MonetaryRevs { get; set; }
        //public int MonetaryRevsId { get; set; }
        public NonMonetaryRevsDTO NonMonetaryRevs { get; set; }
        //public int NonMonetaryRevsId { get; set; }
        public MiscellaneousRevsDTO MiscRevs { get; set; }
        //public int MiscRevsId { get; set; }
        [NotMapped]
        public bool IsCredit { get; set; }

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

        public string OtherRemarks { get; set; }


    }
}
