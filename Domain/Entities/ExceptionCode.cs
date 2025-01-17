using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Infrastructure.Entities;

public partial class ExceptionCode
{
    public int Id { get; set; }

    public string? SubReferenceNo { get; set; }

    public int ExCode { get; set; }

    public string? ExItemRefNo { get; set; }

    public int DeviationStatus { get; set; }

    public DateTime DateCreated { get; set; }

    public int? ApprovalStatus { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? ApprovalRemarks { get; set; }

    public DateTime? TaggingDate { get; set; } = DateTime.Now;
    [JsonIgnore]
    public virtual ExceptionItem? ExItemRefNoNavigation { get; set; }
    [NotMapped]
    public string? ExCodeDescription { get; set; }
    [NotMapped]
    public string? Remarks { get; set; }
}
