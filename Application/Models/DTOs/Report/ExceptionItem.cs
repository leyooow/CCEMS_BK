using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Report
{
    public enum RootCause
    {
        [Display(Name = "Employee Lapse")]
        EmployeeLapses = 1,
        [Display(Name = "Business Decision")]
        BusinessDecision = 2
    }
    public enum AgingCategory
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
    public enum TransactionTypeEnum
    {
        Monetary = 1,
        [Display(Name = "Non Monetary")]
        NonMonetary = 2,
        Miscellaneous = 3
    }
    public enum MainStatus
    {
        Default = -1,
        [Display(Name = "For Approval")]
        PendingApproval = 0,
        Open = 1,
        Closed = 2,
    }
}
