using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Tltxmapping
{
    public int Id { get; set; }

    public double? TltxtranCode { get; set; }

    public string? TltxtranDesc { get; set; }

    public double? TltxrecordNo { get; set; }

    public string? TltxrecordDesc { get; set; }

    public string? AppType { get; set; }

    public string? AccType { get; set; }

    public double? HostTranCode { get; set; }

    public string? HostTranDesc { get; set; }

    public string? DebitCredit { get; set; }

    public double? GlaccountNo { get; set; }

    public string? GlaccountDesc { get; set; }

    public double? AccountNoLoc { get; set; }

    public double? AmountLoc { get; set; }

    public string? AmountComputedLoc { get; set; }
}
