using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Report
{
    public enum ReportStatus
    {
        Standby = 0,
        [Display(Name = "Pending Approval")]
        PendingApproval = 1,
        Approved = 2,
        Sent = 3,
        Closed = 4,
        Rejected = 5
    }

    public enum ReportCoverage
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }

    public enum ReportCategory
    {
        [Display(Name = "Daily Exception Report")]
        DailyExceptionReport = 10,
        [Display(Name = "Red Flag Report")]
        RedFlag = 20,
        [Display(Name = "Escalation Report")]
        Escalation = 30,
        [Display(Name = "Weekly New Accounts Report")]
        NewAccounts = 40,
        [Display(Name = "All Outstanding Exceptions (Monetary & Misc)")]
        AllOutstanding1 = 50,
        [Display(Name = "All Outstanding Exceptions (Non-Monetary)")]
        AllOutstanding2 = 60,
    }

    public enum ReportAdhoc
    {
        Pervasiveness = 1,
        [Display(Name = "Regularization TAT")]
        RegularizationTAT = 2,
        [Display(Name = "Audit Trail")]
        AuditTrail = 3,
        [Display(Name = "Exception Adhocs")]
        ExceptionAdhocs = 4
    }
    public enum DeviationStatus
    {
        //[Display(Name = "Pending Approval")]
        //Pending = 1,
        Outstanding = 2,
        //Resolved = 3,
        [Display(Name = "Tag as Regularized")]
        Regularized = 4,
        [Display(Name = "Tag as Compliance")]
        ForCompliance = 5,
        //Deleted = 10
        [Display(Name = "Tag as Dispensed")]
        Dispensed = 7,
    }

}
