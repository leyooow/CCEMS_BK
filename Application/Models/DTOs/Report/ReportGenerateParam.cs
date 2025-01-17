using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Report
{
    public class ReportGenerateParam
    {
        public Infrastructure.Entities.Report report { get; set; }
        public GenerateMainReportsViewModel GenerateReports { get; set; }
        public int reportCoverage { get; set; }
        public string[] SelectedBranches { get; set; }
         
    }
}
