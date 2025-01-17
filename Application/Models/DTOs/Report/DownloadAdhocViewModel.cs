using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Report
{
    public class DownloadAdhocViewModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public ReportAdhoc ReportAdhoc { get; set; }
        [JsonPropertyName("rt")]
        public RegularizationTAT RT { get; set; }
        [JsonPropertyName("pr")]
        public Pervasiveness PR { get; set; }
        [JsonPropertyName("ea")]
        public ExceptionAdhocs EA { get; set; }

    }

    public class RegularizationTAT
    {
        public int CoveredBranch { get; set; }

        public string EmployeeID { get; set; } = "";
    }

    public class Pervasiveness
    {
        public string EmployeeID { get; set; } = "";
    }


    public class ExceptionAdhocs
    {
        public AdhocStatus ExceptionStatus { get; set; }
    }

    public enum AdhocStatus
    {
        [Display(Name = "Tagged as Regularized")]
        Regularized = 4,
        [Display(Name = "Tagged as For Compliance")]
        ForCompliance = 5,
        Deleted = 6,
        [Display(Name = "Tagged as Dispensed")]
        Dispensed = 7,
    }
    public class AuditTrail
    {

    }
}
