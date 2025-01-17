using System;
using System.Collections.Generic;

namespace Domain.FEntities;

//public partial class BranchCodeTable
//{
//    public string? BrCode { get; set; }

//    public string? BankCode { get; set; }

//    public char? DelFlg { get; set; }

//    public string? MicrCentreCode { get; set; }

//    public string? MicrBankCode { get; set; }

//    public string? MicrBranchCode { get; set; }

//    public string? ClgRepCode { get; set; }

//    public string? CoBrCode { get; set; }

//    public string? DoBrCode { get; set; }

//    public string? BrName { get; set; }

//    public string? BrShortName { get; set; }

//    public string? BrAddr1 { get; set; }

//    public string? BrAddr2 { get; set; }

//    public string? BrCityCode { get; set; }

//    public string? BrStateCode { get; set; }

//    public string? BrPinCode { get; set; }

//    public decimal? BrWklyOff { get; set; }

//    public char? LocalClgFlg { get; set; }

//    public char? SplCollCentreFlg { get; set; }

//    public string? DdIssBrCode { get; set; }

//    public string? DdIssBrName { get; set; }

//    public string? CollAgentBrCode { get; set; }

//    public char? InwardTtFlg { get; set; }

//    public char? OutwardTtFlg { get; set; }

//    public string? LchgUserId { get; set; }

//    public DateTime? LchgTime { get; set; }

//    public string? RcreUserId { get; set; }

//    public DateTime? RcreTime { get; set; }

//    public string? RoutingBrCode { get; set; }

//    public string? BcbCode { get; set; }

//    public string? CollAgentBankCode { get; set; }

//    public string? BrAddr3 { get; set; }

//    public string? CntryCode { get; set; }

//    public string? TlxNum { get; set; }

//    public string? PhoneNum { get; set; }

//    public string? FaxNum { get; set; }

//    public string? BankBrRmks { get; set; }

//    public string? OurOfficeCode { get; set; }

//    public string? OurOfficeBankCode { get; set; }

//    public char? FrnBrFlg { get; set; }

//    public string? RoutingBankCode { get; set; }

//    public char? FcIbtAlwdFlg { get; set; }

//    public decimal? DaysForValueDate { get; set; }

//    public char? TradeFinanceBranch { get; set; }

//    public decimal? TsCnt { get; set; }

//    public string? DcAlias { get; set; }

//    public string? ClgRefCode { get; set; }

//    public string? FreeCode1 { get; set; }

//    public string? FreeCode2 { get; set; }

//    public string? FreeCode3 { get; set; }

//    public string? BrFreeCode1 { get; set; }

//    public string? BrFreeCode2 { get; set; }

//    public string? BrFreeCode3 { get; set; }

//    public string? BrFreeCode4 { get; set; }

//    public string? BrFreeCode5 { get; set; }

//    public string? BrFreeCode6 { get; set; }

//    public string? BrFreeCode7 { get; set; }

//    public string? BrFreeCode8 { get; set; }

//    public string? BrFreeCode9 { get; set; }

//    public string? BrFreeCode10 { get; set; }

//    public string? IsoCntryCode { get; set; }

//    public char? CtsEnabledFlg { get; set; }

//    public string? PrefLangCode { get; set; }

//    public string? PrefLangBrName { get; set; }

//    public string? PrefLangBrShortName { get; set; }

//    public string? PrefLangBrAddr1 { get; set; }

//    public string? PrefLangBrAddr2 { get; set; }

//    public string? Bic { get; set; }

//    public string? LocalBankCode { get; set; }

//    public string? BankId { get; set; }

//    public string? Alt1BrName { get; set; }

//    public string? Alt1BrShortName { get; set; }

//    public string? OtherEntityBankId { get; set; }

//    public string? PrefLangBrAddr3 { get; set; }

//    public string? SwiftBrName { get; set; }

//    public string? SwiftBrAddr1 { get; set; }

//    public string? SwiftBrAddr2 { get; set; }

//    public string? SwiftBrAddr3 { get; set; }

//    public string? Alt1LangBrAddr1 { get; set; }

//    public string? Alt1LangBrAddr2 { get; set; }

//    public string? Alt1LangBrAddr3 { get; set; }

//    public string? FactorCode { get; set; }
//}


//using System;

//public class BranchCodeTable
//{
//    public string BrCode { get; set; }
//    public string BankCode { get; set; }
//    public string DelFlg { get; set; }
//    public string MicrCentreCode { get; set; }
//    public string MicrBankCode { get; set; }
//    public string MicrBranchCode { get; set; }
//    public string ClgRepCode { get; set; }
//    public string CoBrCode { get; set; }
//    public string DoBrCode { get; set; }
//    public string BrName { get; set; }
//    public string BrShortName { get; set; }
//    public string BrAddr1 { get; set; }
//    public string BrAddr2 { get; set; }
//    public string BrCityCode { get; set; }
//    public string BrStateCode { get; set; }
//    public string BrPinCode { get; set; }
//    public int? BrWklyOff { get; set; }
//    public string LocalClgFlg { get; set; }
//    public string SplCollCentreFlg { get; set; }
//    public string DdIssBrCode { get; set; }
//    public string DdIssBrName { get; set; }
//    public string CollAgentBrCode { get; set; }
//    public string InwardTtFlg { get; set; }
//    public string OutwardTtFlg { get; set; }
//    public string LchgUserId { get; set; }
//    public DateTime? LchgTime { get; set; }
//    public string RcreUserId { get; set; }
//    public DateTime? RcreTime { get; set; }
//    public string RoutingBrCode { get; set; }
//    public string BcbCode { get; set; }
//    public string CollAgentBankCode { get; set; }
//    public string BrAddr3 { get; set; }
//    public string CntryCode { get; set; }
//    public string TlxNum { get; set; }
//    public string PhoneNum { get; set; }
//    public string FaxNum { get; set; }
//    public string BankBrRmks { get; set; }
//    public string OurOfficeCode { get; set; }
//    public string OurOfficeBankCode { get; set; }
//    public string FrnBrFlg { get; set; }
//    public string RoutingBankCode { get; set; }
//    public string FcIbtAlwdFlg { get; set; }
//    public int? DaysForValueDate { get; set; }
//    public string TradeFinanceBranch { get; set; }
//    public int? TsCnt { get; set; }
//    public string DcAlias { get; set; }
//    public string ClgRefCode { get; set; }
//    public string FreeCode1 { get; set; }
//    public string FreeCode2 { get; set; }
//    public string FreeCode3 { get; set; }
//    public string BrFreeCode1 { get; set; }
//    public string BrFreeCode2 { get; set; }
//    public string BrFreeCode3 { get; set; }
//    public string BrFreeCode4 { get; set; }
//    public string BrFreeCode5 { get; set; }
//    public string BrFreeCode6 { get; set; }
//    public string BrFreeCode7 { get; set; }
//    public string BrFreeCode8 { get; set; }
//    public string BrFreeCode9 { get; set; }
//    public string BrFreeCode10 { get; set; }
//    public string IsoCntryCode { get; set; }
//    public string CtsEnabledFlg { get; set; }
//    public string PrefLangCode { get; set; }
//    public string PrefLangBrName { get; set; }
//    public string PrefLangBrShortName { get; set; }
//    public string PrefLangBrAddr1 { get; set; }
//    public string PrefLangBrAddr2 { get; set; }
//    public string Bic { get; set; }
//    public string LocalBankCode { get; set; }
//    public string BankId { get; set; }
//    public string Alt1BrName { get; set; }
//    public string Alt1BrShortName { get; set; }
//    public string OtherEntityBankId { get; set; }
//    public string PrefLangBrAddr3 { get; set; }
//    public string SwiftBrName { get; set; }
//    public string SwiftBrAddr1 { get; set; }
//    public string SwiftBrAddr2 { get; set; }
//    public string SwiftBrAddr3 { get; set; }
//    public string Alt1LangBrAddr1 { get; set; }
//    public string Alt1LangBrAddr2 { get; set; }
//    public string Alt1LangBrAddr3 { get; set; }
//    public string FactorCode { get; set; }
//}

public class BranchCodeTable
{
    public string BrCode { get; set; }
    public string BrName { get; set; }
}