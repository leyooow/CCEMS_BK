using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class ExceptionDeviationList
{
    public Guid Id { get; set; }

    public string? Classification { get; set; }

    public string? Deviation { get; set; }

    public string? RiskClassification { get; set; }

    public string? RiskAssessment { get; set; }

    public string? LikelihoodScore { get; set; }

    public string? LikelihoodDesc { get; set; }

    public string? MagnitudeScore { get; set; }

    public string? MagnitudeDesc { get; set; }

    public string? RiskAssessmentScore { get; set; }

    public DateTime? EntryDate { get; set; }
}
