using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Report
{
    public class OutstandingDaily
    {
        [DataMember(Order = 0)]
        public string ExceptionNo { get; set; }
        [DataMember(Order = 1)]
        public string BranchCode { get; set; }
        [DataMember(Order = 2)]
        public string BranchName { get; set; }
        [DataMember(Order = 3)]
        public string Area { get; set; }
        [DataMember(Order = 4)]
        public string Division { get; set; }
        [DataMember(Order = 5)]
        public string TransactionDate { get; set; }
        [DataMember(Order = 6)]
        public string Aging { get; set; }
        [DataMember(Order = 7)]
        public AgingCategory AgingCategory { get; set; }
        [DataMember(Order = 8)]
        public string Process { get; set; }
        [DataMember(Order = 9)]
        public string AccountNo { get; set; }
        [DataMember(Order = 10)]
        public string AccountName { get; set; }
        [DataMember(Order = 11)]
        public string Deviation { get; set; }
        [DataMember(Order = 12)]
        public string RiskClassification { get; set; }
        [DataMember(Order = 13)]
        public string DeviationCategory { get; set; }
        [DataMember(Order = 14)]
        public decimal Amount { get; set; }
        [DataMember(Order = 15)]
        public string PersonResponsible { get; set; }
        [DataMember(Order = 16)]
        public string OtherPersonResponsible { get; set; }
        [DataMember(Order = 17)]
        public string Remarks { get; set; }
        [DataMember(Order = 18)]
        public string ActionPlan { get; set; }
        [DataMember(Order = 19)]
        public string EncodedBy { get; set; }
        [DataMember(Order = 20)]
        public RootCause RootCause { get; set; }
        [DataMember(Order = 21)]
        public string DeviationApprover { get; set; }
    }

    public class RegularizedDaily
    {
        [DataMember(Order = 0)]
        public string ExceptionNo { get; set; }
        [DataMember(Order = 1)]
        public string BranchCode { get; set; }
        [DataMember(Order = 2)]
        public string BranchName { get; set; }
        [DataMember(Order = 3)]
        public string Area { get; set; }
        [DataMember(Order = 4)]
        public string Division { get; set; }
        [DataMember(Order = 5)]
        public string TransactionDate { get; set; }
        [DataMember(Order = 6)]
        public string ChangesDateApproval { get; set; }
        [DataMember(Order = 7)]
        public string Process { get; set; }

        [DataMember(Order = 8)]
        public string AccountNo { get; set; }
        [DataMember(Order = 9)]
        public string AccountName { get; set; }
        [DataMember(Order = 10)]
        public string Deviation { get; set; }
        [DataMember(Order = 11)]
        public string Category { get; set; }
        [DataMember(Order = 12)]
        public decimal Amount { get; set; }
        [DataMember(Order = 13)]
        public string PersonResponsible { get; set; }
        [DataMember(Order = 14)]
        public string OtherPersonResponsible { get; set; }
        [DataMember(Order = 15)]
        public string Remarks { get; set; }
        [DataMember(Order = 16)]
        public string ActionPlan { get; set; }
        [DataMember(Order = 17)]
        public string EncodedBy { get; set; }

    }

    public class RedFlagDaily
    {
        [DataMember(Order = 0)]
        public string ExceptionNo { get; set; }
        [DataMember(Order = 1)]
        public string BranchCode { get; set; }
        [DataMember(Order = 2)]
        public string BranchName { get; set; }
        [DataMember(Order = 3)]
        public string Area { get; set; }
        [DataMember(Order = 4)]
        public string Division { get; set; }
        [DataMember(Order = 5)]
        public string TransactionDate { get; set; }
        [DataMember(Order = 6)]
        public string Aging { get; set; }
        [DataMember(Order = 7)]
        public AgingCategory AgingCategory { get; set; }
        [DataMember(Order = 8)]
        public string Process { get; set; }
        [DataMember(Order = 9)]
        public string AccountNo { get; set; }
        [DataMember(Order = 10)]
        public string AccountName { get; set; }
        [DataMember(Order = 11)]
        public string Deviation { get; set; }
        [DataMember(Order = 12)]
        public string RiskClassification { get; set; }
        [DataMember(Order = 13)]
        public string DeviationCategory { get; set; }
        [DataMember(Order = 14)]
        public decimal Amount { get; set; }
        [DataMember(Order = 15)]
        public string PersonResponsible { get; set; }
        [DataMember(Order = 16)]
        public string OtherPersonResponsible { get; set; }
        [DataMember(Order = 17)]
        public string Remarks { get; set; }
        [DataMember(Order = 18)]
        public string ActionPlan { get; set; }
        [DataMember(Order = 19)]
        public string EncodedBy { get; set; }
        [DataMember(Order = 20)]
        public RootCause RootCause { get; set; }
        [DataMember(Order = 21)]
        public string DeviationApprover { get; set; }

    }

    public class DeletedDaily
    {
        [DataMember(Order = 0)]
        public string ExceptionNo { get; set; }
        [DataMember(Order = 1)]
        public string BranchCode { get; set; }
        [DataMember(Order = 2)]
        public string BranchName { get; set; }
        [DataMember(Order = 3)]
        public string Area { get; set; }
        [DataMember(Order = 4)]
        public string Division { get; set; }
        [DataMember(Order = 5)]
        public string TransactionDate { get; set; }
        [DataMember(Order = 6)]
        public string ChangesDateApproval { get; set; }
        [DataMember(Order = 7)]
        public string Process { get; set; }

        [DataMember(Order = 8)]
        public string AccountNo { get; set; }
        [DataMember(Order = 9)]
        public string AccountName { get; set; }
        [DataMember(Order = 10)]
        public string Deviation { get; set; }
        [DataMember(Order = 11)]
        public string Category { get; set; }
        [DataMember(Order = 12)]
        public decimal Amount { get; set; }
        [DataMember(Order = 13)]
        public string PersonResponsible { get; set; }
        [DataMember(Order = 14)]
        public string OtherPersonResponsible { get; set; }
        [DataMember(Order = 15)]
        public string Remarks { get; set; }
        [DataMember(Order = 16)]
        public string ActionPlan { get; set; }
        [DataMember(Order = 17)]
        public string EncodedBy { get; set; }

    }

    public class NewAccountsWeekly
    {
        [DataMember(Order = 0)]
        public string ExceptionNo { get; set; }
        [DataMember(Order = 1)]
        public string BranchCode { get; set; }
        [DataMember(Order = 2)]
        public string BranchName { get; set; }
        [DataMember(Order = 3)]
        public string Area { get; set; }
        [DataMember(Order = 4)]
        public string Division { get; set; }
        [DataMember(Order = 5)]
        public string TransactionDate { get; set; }
        [DataMember(Order = 6)]
        public string Aging { get; set; }
        [DataMember(Order = 7)]
        public AgingCategory AgingCategory { get; set; }
        [DataMember(Order = 8)]
        public string Process { get; set; }
        [DataMember(Order = 9)]
        public string AccountNo { get; set; }
        [DataMember(Order = 10)]
        public string AccountName { get; set; }
        [DataMember(Order = 11)]
        public string Deviation { get; set; }
        [DataMember(Order = 12)]
        public string RiskClassification { get; set; }
        [DataMember(Order = 13)]
        public string DeviationCategory { get; set; }
        [DataMember(Order = 14)]
        public decimal Amount { get; set; }
        [DataMember(Order = 15)]
        public string PersonResponsible { get; set; }
        [DataMember(Order = 16)]
        public string OtherPersonResponsible { get; set; }
        [DataMember(Order = 17)]
        public string Remarks { get; set; }
        [DataMember(Order = 18)]
        public string ActionPlan { get; set; }
        [DataMember(Order = 19)]
        public string EncodedBy { get; set; }
        [DataMember(Order = 20)]
        public RootCause RootCause { get; set; }
        [DataMember(Order = 21)]
        public string DeviationApprover { get; set; }
    }

    public class EscalationWeekly
    {
        [DataMember(Order = 0)]
        public string ExceptionNo { get; set; }
        [DataMember(Order = 1)]
        public string BranchCode { get; set; }
        [DataMember(Order = 2)]
        public string BranchName { get; set; }
        [DataMember(Order = 3)]
        public string Area { get; set; }
        [DataMember(Order = 4)]
        public string Division { get; set; }
        [DataMember(Order = 5)]
        public string TransactionDate { get; set; }
        [DataMember(Order = 6)]
        public string Aging { get; set; }
        [DataMember(Order = 7)]
        public string AgingCategory { get; set; }
        [DataMember(Order = 8)]
        public string Process { get; set; }
        [DataMember(Order = 9)]
        public string AccountNo { get; set; }
        [DataMember(Order = 10)]
        public string AccountName { get; set; }
        [DataMember(Order = 11)]
        public string Deviation { get; set; }
        [DataMember(Order = 12)]
        public string RiskClassification { get; set; }
        [DataMember(Order = 13)]
        public string DeviationCategory { get; set; }
        [DataMember(Order = 14)]
        public decimal Amount { get; set; }
        [DataMember(Order = 15)]
        public string PersonResponsible { get; set; }
        [DataMember(Order = 16)]
        public string OtherPersonResponsible { get; set; }
        [DataMember(Order = 17)]
        public string Remarks { get; set; }
        [DataMember(Order = 18)]
        public string ActionPlan { get; set; }
        [DataMember(Order = 19)]
        public string EncodedBy { get; set; }
        [DataMember(Order = 20)]
        public string RootCause { get; set; }
        [DataMember(Order = 21)]
        public string DeviationApprover { get; set; }
    }

    public class AllOutstandingMonthly
    {
        [DataMember(Order = 0)]
        public string ExceptionNo { get; set; }
        [DataMember(Order = 1)]
        public string BranchCode { get; set; }
        [DataMember(Order = 2)]
        public string BranchName { get; set; }
        [DataMember(Order = 3)]
        public string Area { get; set; }
        [DataMember(Order = 4)]
        public string Division { get; set; }
        [DataMember(Order = 5)]
        public string TransactionDate { get; set; }
        [DataMember(Order = 6)]
        public string Aging { get; set; }
        [DataMember(Order = 7)]
        public string AgingCategory { get; set; }
        [DataMember(Order = 8)]
        public string Process { get; set; }
        [DataMember(Order = 9)]
        public string AccountNo { get; set; }
        [DataMember(Order = 10)]
        public string AccountName { get; set; }
        [DataMember(Order = 11)]
        public string Deviation { get; set; }
        [DataMember(Order = 12)]
        public string RiskClassification { get; set; }
        [DataMember(Order = 13)]
        public string DeviationCategory { get; set; }
        [DataMember(Order = 14)]
        public decimal Amount { get; set; }
        [DataMember(Order = 15)]
        public string PersonResponsible { get; set; }
        [DataMember(Order = 16)]
        public string OtherPersonResponsible { get; set; }
        [DataMember(Order = 17)]
        public string Remarks { get; set; }
        [DataMember(Order = 18)]
        public string ActionPlan { get; set; }
        [DataMember(Order = 19)]
        public string EncodedBy { get; set; }
        [DataMember(Order = 20)]
        public string RootCause { get; set; }
        [DataMember(Order = 21)]
        public string DeviationApprover { get; set; }
    }

    public class AdhocsOthers
    {
        [DataMember(Order = 0)]
        public string ExceptionNo { get; set; }
        [DataMember(Order = 1)]
        public string BranchCode { get; set; }
        [DataMember(Order = 2)]
        public string BranchName { get; set; }
        [DataMember(Order = 3)]
        public string Area { get; set; }
        [DataMember(Order = 4)]
        public string Division { get; set; }
        [DataMember(Order = 5)]
        public string TransactionDate { get; set; }
        [DataMember(Order = 6)]
        public string DateModified { get; set; }
        [DataMember(Order = 7)]
        public string Aging { get; set; }
        [DataMember(Order = 8)]
        public string AgingCategory { get; set; }
        [DataMember(Order = 9)]
        public string Process { get; set; }
        [DataMember(Order = 10)]
        public string AccountNo { get; set; }
        [DataMember(Order = 11)]
        public string AccountName { get; set; }
        [DataMember(Order = 12)]
        public string Deviation { get; set; }
        [DataMember(Order = 13)]
        public string RiskClassification { get; set; }
        [DataMember(Order = 14)]
        public string DeviationCategory { get; set; }
        [DataMember(Order = 15)]
        public decimal Amount { get; set; }
        [DataMember(Order = 16)]
        public string PersonResponsible { get; set; }
        [DataMember(Order = 17)]
        public string OtherPersonResponsible { get; set; }
        [DataMember(Order = 18)]
        public string Remarks { get; set; }
        [DataMember(Order = 19)]
        public string ActionPlan { get; set; }
        [DataMember(Order = 20)]
        public string EncodedBy { get; set; }
        [DataMember(Order = 21)]
        public string RootCause { get; set; }
        [DataMember(Order = 22)]
        public string DeviationApprover { get; set; }
    }

    public class AdhocsAllDeleted
    {
        [DataMember(Order = 0)]
        public string ExceptionNo { get; set; }
        [DataMember(Order = 1)]
        public string BranchCode { get; set; }
        [DataMember(Order = 2)]
        public string BranchName { get; set; }
        [DataMember(Order = 3)]
        public string Area { get; set; }
        [DataMember(Order = 4)]
        public string Division { get; set; }
        [DataMember(Order = 5)]
        public string TransactionDate { get; set; }
        [DataMember(Order = 6)]
        public string DeletionDate { get; set; }
        [DataMember(Order = 7)]
        public string Aging { get; set; }
        [DataMember(Order = 8)]
        public string AgingCategory { get; set; }
        [DataMember(Order = 9)]
        public string Process { get; set; }
        [DataMember(Order = 10)]
        public string AccountNo { get; set; }
        [DataMember(Order = 11)]
        public string AccountName { get; set; }
        [DataMember(Order = 12)]
        public string Deviation { get; set; }
        [DataMember(Order = 13)]
        public string RiskClassification { get; set; }
        [DataMember(Order = 14)]
        public string DeviationCategory { get; set; }
        [DataMember(Order = 15)]
        public decimal Amount { get; set; }
        [DataMember(Order = 16)]
        public string PersonResponsible { get; set; }
        [DataMember(Order = 17)]
        public string OtherPersonResponsible { get; set; }
        [DataMember(Order = 18)]
        public string Remarks { get; set; }
        [DataMember(Order = 19)]
        public string DeletionRemarks { get; set; }
        [DataMember(Order = 20)]
        public string ActionPlan { get; set; }
        [DataMember(Order = 21)]
        public string EncodedBy { get; set; }
        [DataMember(Order = 22)]
        public string RootCause { get; set; }
        [DataMember(Order = 23)]
        public string DeviationApprover { get; set; }
    }

    public class RegularizationTATFormat
    {
        public string PersonResponsibleID { get; set; }
        public string PersonResponsibleName { get; set; }
        //public string OtherPersonResponsibleName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ExceptionNo { get; set; }
        public string SubExceptionNo { get; set; }
        public string Deviation { get; set; }
        public string DeviationCategory { get; set; }
        public string TransactionDate { get; set; }
        public string RegularizeDate { get; set; }
        public double DaysOutstanding { get; set; }
        public DeviationStatus Status { get; set; }
    }

    public class PervasivenessFormat
    {
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ExceptionNo { get; set; }
        public string SubExceptionNo { get; set; }
        public string Deviation { get; set; }
        public string DeviationCategory { get; set; }
        public string TimesComitted { get; set; }
        public string DateRecorded { get; set; }
        public DeviationStatus Status { get; set; }
    }

    public class RegularReportFormat
    {
        [DataMember(Order = 0)]
        public string ExceptionNo { get; set; }
        [DataMember(Order = 1)]
        public string BranchCode { get; set; }
        [DataMember(Order = 2)]
        public string BranchName { get; set; }
        [DataMember(Order = 3)]
        public string Area { get; set; }
        [DataMember(Order = 4)]
        public string Division { get; set; }
        [DataMember(Order = 5)]
        public string TransactionDate { get; set; }
        [DataMember(Order = 6)]
        public string Aging { get; set; }
        [DataMember(Order = 7)]
        public AgingCategory AgingCategory { get; set; }
        [DataMember(Order = 8)]
        public string Process { get; set; }
        [DataMember(Order = 9)]
        public string AccountNo { get; set; }
        [DataMember(Order = 10)]
        public string AccountName { get; set; }
        [DataMember(Order = 11)]
        public string Deviation { get; set; }
        [DataMember(Order = 12)]
        public string RiskClassification { get; set; }
        [DataMember(Order = 13)]
        public string DeviationCategory { get; set; }
        [DataMember(Order = 14)]
        public decimal Amount { get; set; }
        [DataMember(Order = 15)]
        public string PersonResponsible { get; set; }
        [DataMember(Order = 16)]
        public string OtherPersonResponsible { get; set; }
        [DataMember(Order = 17)]
        public string Remarks { get; set; }
        [DataMember(Order = 18)]
        public string ActionPlan { get; set; }
        [DataMember(Order = 19)]
        public string EncodedBy { get; set; }
        [DataMember(Order = 20)]
        public RootCause RootCause { get; set; }
        [DataMember(Order = 21)]
        public string DeviationApprover { get; set; }
    }
}
