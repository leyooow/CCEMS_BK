using Application.Models.Helpers;
using Infrastructure.Entities;

namespace Application.Contracts.Repositories
{
    public interface IReportDetailsRepository
    {
        Task<string[]> GetBranchNames(List<string> br);
        Task<PaginatedList<ReportContent>> GetReportContentsList(int id, int page);
        Task<List<ReportContent>> GetReportContentsList(int id);
        Task<List<ReportContent>> GetReportContentsList(int id, string refno);
        Task<List<BranchReply>> GetBranchReplyList(string ExceptionNo, Guid Id);
        Task<Report> GetReports(int id);
        Task<List<string>> GetBranchAccesses(int[] intBranchCodes);
        Task<List<User>> GetUsersByBranch(List<string> branchEmp);
        Task<ReportsRev> GetReportsRevsLast(Report report);
        Task<EPPlusReturn> GeneratePulloutRequest(string UserName, List<ReportContent> rContents, Report report, ReportsRev reportRevs, List<string> footerContent);
        Task<User> GetApprover(string ApprovedBy);
        Task<string> GetApprovBy(int id);
        Task<User> GetMaker(string CreatedBy);
        Task<List<ReportContent>> GetRiskAssesmentAsync(List<ReportContent> data);
        Task<EPPlusReturn> ExportDataFromDetailsEscalation(List<ReportContent> rContents, Report report, List<string> footerContent, string approvedBy, string UserName, int id);
        EPPlusReturn ExportDataFromDetailsRedFlag(List<ReportContent> rContents, Report report, List<string> footerContent, string approvedBy, string UserName);
        EPPlusReturn ExportDataFromDetailsDailyExceptionReport(List<ReportContent> rContents, Report report, List<string> footerContent, string approvedBy, string UserName);
        EPPlusReturn ExportDataFromDetailsNewAccounts(List<ReportContent> rContents, Report report, List<string> footerContent, string approvedBy, string UserName);
        EPPlusReturn ExportDataFromDetailsAllOutstanding1(List<ReportContent> rContents, Report report, List<string> footerContent, string approvedBy, string UserName);
        EPPlusReturn ExportDataFromDetailsAllOutstanding2(List<ReportContent> rContents, Report report, List<string> footerContent, string approvedBy, string UserName);
        Task<ReportsRev> GetReportsRevs(Guid reportsGuid);
        Task UpdateReportAndRev(Report report, ReportsRev reportRevs);
        Task UpdateReport(Report report);
        Task UpdateReportRev(ReportsRev reportRevs);
        Task<User> GetUserEmployeeID(string LoginName);
        void SendMakerNotification(string[] contentDetails, string recipient, bool isApproved);
        void SendBranchNotification(string[] contentDetailsBranch, string[] toEmployees, string[] ccEmployees);
        void SendApproverNotification(string[] contentDetails, List<string> recipients);
        string GetHostConfig();
        string GetGetDisplayNameReportCategory(int ReportCategory);
        Task<List<string>> GetRecipients(Report query);

    }
}
