using Application.Models.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Entities;

namespace Application.Models.DTOs.Report
{
    public class GenerateMainReportsViewModel
    {
        public ReportCategory ReportCategory { get; set; }
        public ReportCoverage ReportCoverage {  get; set; }
        public DailyCategory DailyCategory { get; set; }
        public WeeklyCategory WeeklyCategory { get; set; }
        public MonthlyCategory MonthlyCategory { get; set; }
        public RegularReports RegularReportName { get; set; }
        public List<string> SelectedBranches { get; set; }

        [FutureDatedValidation]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCoverage { get; set; }

        [DateRangeValidation]
        [FutureDatedValidation]
        [DataType(DataType.Date)]
        public DateTime DateFrom { get; set; }

        [DateRangeValidation]
        [FutureDatedValidation]
        [DataType(DataType.Date)]
        public DateTime DateTo { get; set; }
    }

    public enum DailyCategory
    {
        [Display(Name = "Daily Exception Report")]
        DailyExceptionReport = 10,
        [Display(Name = "Red Flag Report")]
        RedFlag = 20,
        //Regularized = 3,
    }

    public enum WeeklyCategory
    {
        //Outstanding = 1,
        [Display(Name = "Escalation Report")]
        Escalation = 30,
        [Display(Name = "Weekly New Accounts Report")]
        NewAccounts = 40,
    }

    public enum MonthlyCategory
    {
        [Display(Name = "All Outstanding Exceptions (Monetary & Misc)")]
        AllOutstanding1 = 50,
        [Display(Name = "All Outstanding Exceptions (Non-Monetary)")]
        AllOutstanding2 = 60,
        //Regularized = 4,
    }

    public enum RegularReports
    {
        [Display(Name = "Daily Exception Report")]
        DailyExceptionReport,
        [Display(Name = "Weekly New Accounts Report")]
        NewAccountsReport,
        [Display(Name = "Red Flag Report")]
        RedFlagReport,
        [Display(Name = "All Outstanding Exception Report (Monetary & Misc)")]
        AllOutstandingReport1,
        [Display(Name = "All Outstanding Exception Report (Non-Monetary)")]
        AllOutstandingReport2,
        [Display(Name = "Escalation Report")]
        EscalationReport
    }
}
