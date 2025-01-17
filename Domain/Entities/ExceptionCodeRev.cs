using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Infrastructure.Entities;

public partial class ExceptionCodeRev
{
    public int Id { get; set; }

    public string? SubReferenceNo { get; set; }

    public int ExCode { get; set; }

    public Guid? ExItemId { get; set; }

    public string? ExItemRefNo { get; set; }

    public int DeviationStatus { get; set; }

    public string? ActionTaken { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDateTime { get; set; }

    public string? Changes { get; set; }

    public bool IsProcessed { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public string Remarks { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public int? ApprovalStatus { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? ApprovalRemarks { get; set; }

    public DateTime TaggingDate { get; set; }
    [JsonIgnore]
    public virtual ExceptionItemRev? ExItem { get; set; }
    [NotMapped]
    public string ExCodeDescription { get; set; }
}
