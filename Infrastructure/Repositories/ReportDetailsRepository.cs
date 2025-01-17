using Application.Models.Helpers;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Entities;
using Application.Contracts.Repositories;
using OfficeOpenXml;
using Infrastructure.Helpers;
using Application.Models.DTOs.Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Application.Models.Responses;
using System.Security.Policy;
using System.Linq;
using Org.BouncyCastle.Cms;

namespace Infrastructure.Repositories
{
    public class ReportDetailsRepository : IReportDetailsRepository
    {
        private readonly CcemQatContext _context;
        private readonly EPPlusPackages _package;
        private Utilities _util;
        private Notification _emailNotif;
        public ReportDetailsRepository(CcemQatContext context, EPPlusPackages package, Utilities util, Notification emailNotif) 
        { 
            _context = context;
            _package = package;
            _util = util;
            _emailNotif = emailNotif;
        }
        public async Task<string[]> GetBranchNames(List<string> br)
        {
            return await _context.Groups.Where(s => br.Contains(s.Code)).Select(s => s.Name).ToArrayAsync();
        }
        public async Task<PaginatedList<ReportContent>> GetReportContentsList(int id, int page)
        {
            var rContents = _context.ReportContents.Where(s => s.ReportId == id);
            return await PaginatedList<ReportContent>.CreateAsync(rContents, page);
        }
        public async Task<List<ReportContent>> GetReportContentsList(int id)
        {
            return await _context.ReportContents.Where(s => s.ReportId == id).ToListAsync();
        }
        public async Task<List<ReportContent>> GetReportContentsList(int id,string refno)
        {
            return await _context.ReportContents.Where(s => s.ReportId == id && s.ExceptionNo == refno).ToListAsync();
        }
        public async Task<List<BranchReply>> GetBranchReplyList(string ExceptionNo, Guid Id)
        {
            return await _context.BranchReplies.Where(x => x.ExceptionNo == ExceptionNo && x.ReportContentsId == Id).ToListAsync();
        }
        public async Task<Report> GetReports(int id)
        {
            return await _context.Reports.SingleOrDefaultAsync(s => s.Id == id);
        }
        public async Task<List<string>> GetBranchAccesses(int[] intBranchCodes)
        {
            return await _context.BranchAccesses.Where(s => intBranchCodes.Contains(s.BranchId)).Select(s => s.EmployeeId).Distinct().ToListAsync();
        }
        public async Task<List<User>> GetUsersByBranch(List<string> branchEmp)
        {
            return await _context.Users.Where(s => branchEmp.Contains(s.EmployeeId)).ToListAsync();
        }
        public async Task<ReportsRev> GetReportsRevsLast(Report report)
        {
            return await _context.ReportsRevs
                        .Where(s => s.ReportsGuid == report.ReportsGuid)
                        .OrderBy(x => x.DateGenerated)
                        .LastOrDefaultAsync() ?? new ReportsRev();
        }
        public async Task<EPPlusReturn> GeneratePulloutRequest(string UserName, List<ReportContent> rContents, Report report, ReportsRev reportRevs, List<string> footerContent)
        {
            try
            {

                byte[] excelData;
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = string.Empty;
                string folderPath = Settings.Config.PulloutRequestSourceFolder.ToString(); // Specify the folder path
                string folderPathTemp = Settings.Config.PulloutRequestTempFile.ToString(); // Specify the folder path

                string[] myFiles = Directory.GetFiles(folderPathTemp);
                foreach (string res in myFiles)
                {
                    FileInfo file = new FileInfo(res);
                    if (file.Name == "Pulloutrequest.xlsx" || file.Name == "TempPulloutrequest.xlsx")
                    {
                        //do nothing
                    }
                    else
                    {
                        file.Delete();
                    }
                }

                FileInfo source = new FileInfo(Settings.Config.PulloutRequestSourceFile.ToString());
                FileInfo output;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(source))
                {


                    Random rnd = new Random();
                    int seqNo = rnd.Next();

                    fileName = string.Format("{0}_{1}{2}{3}.xlsx",
                         "PulloutRequest", DateTime.Now.ToString("yyyy"),
                         _util.PaddingFieldValue(rContents.Take(1).Single().BranchCode, false,
                         3, "0"),
                         seqNo.ToString().Substring(0, 3));



                    excelData = _package.PulloutRequestPackage(rContents, report, package, footerContent, UserName, "", seqNo.ToString().Substring(0, 3));

                    // Save Excel file to a specific folder on the server
                    string filePath = Path.Combine(folderPathTemp, fileName);

                    System.IO.File.WriteAllBytes(filePath, excelData);

                    var getEmailRecipient = await (from a in _context.BranchAccesses
                                                   from b in _context.Users
                                                   where a.EmployeeId == b.EmployeeId
                                                   where a.BranchId == Convert.ToInt16(reportRevs.SelectedBranches)
                                                   where b.RoleId == Convert.ToInt16(Settings.Config.PulloutRequestRoleID.ToString())
                                                   select b).ToListAsync();

                    foreach (var item in getEmailRecipient)
                    {
                        // Send email with Excel file attachment
                        EmailNotifPullout(item.Email, filePath);
                    }

                    return new EPPlusReturn { FileByte = excelData, ContentType = contentType, FileName = fileName };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void EmailNotifPullout(string email, string fileName)
        {
            _emailNotif.PulloutNotification(email, fileName);
        }
        public async Task<User> GetApprover(string ApprovedBy)
        {
            return await _context.Users.Include(s => s.Role).SingleOrDefaultAsync(s => s.LoginName == ApprovedBy) ;
        }
        public async Task<string> GetApprovBy(int id)
        {
            var x = await (from a in _context.Reports
                           from b in _context.ReportsRevs
                           where a.Id == id
                           where b.ReportsGuid == a.ReportsGuid
                           orderby b.Id descending
                           select b).Take(1).SingleAsync();
            if(x != null)
                return x.ApprovedBy ?? "";
            return "";
        }
        public async Task<User> GetMaker(string CreatedBy)
        {
            return await _context.Users.Include(s => s.Role).SingleOrDefaultAsync(s => s.LoginName == CreatedBy);
        }
        public async Task<List<ReportContent>> GetRiskAssesmentAsync(List<ReportContent> data)
        {
            try
            {

                var list = data;
                var getData = await _context.ExceptionDeviationLists.ToListAsync();

                foreach (var item in data)
                {
                    var test = getData.Where(x => x.Deviation.Replace('"', ' ').Trim().ToLower()
                                == item.Deviation.Replace('"', ' ').Trim().ToLower())
                        .ToList();

                    item.Deviation += ":" + test.Last().RiskAssessment;

                }
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<EPPlusReturn> ExportDataFromDetailsEscalation(List<ReportContent> rContents, Report report, List<string> footerContent,string approvedBy, string UserName, int id)
        {
            try
            {
                EPPlusReturn result = new();
                using (ExcelPackage package = new ExcelPackage())
                {
                    var getAging = Settings.Config.IncludeOnlyEscalationReportAgingDays.Split(',');
                    rContents = await _context.ReportContents.Where(s => s.ReportId == id && getAging.Contains(s.Aging)).ToListAsync(); ;

                    result.FileByte = _package.WeeklyEscalationPackage(rContents, report, package, footerContent, UserName, approvedBy);
                    result.FileName = string.Format("{1}_{0}.xlsx", DateTime.Now.ToString("MM-dd-yyyy"), "Escalation");
                }
                return result;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public EPPlusReturn ExportDataFromDetailsRedFlag(List<ReportContent> rContents, Report report, List<string> footerContent, string approvedBy, string UserName)
        {
            try
            {
                EPPlusReturn result = new();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    result.FileByte = _package.DailyRedFlagPackage(rContents, report, package, footerContent, UserName, approvedBy);
                    result.FileName = string.Format("{1}_{0}.xlsx", DateTime.Now.ToString("MM-dd-yyyy"), "DailyRedFlag");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EPPlusReturn ExportDataFromDetailsDailyExceptionReport(List<ReportContent> rContents, Report report, List<string> footerContent, string approvedBy, string UserName)
        {
            try
            {
                EPPlusReturn result = new();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    result.FileByte = _package.DailyOutstandingPackage(rContents, report, package, footerContent, UserName, approvedBy);
                    result.FileName = string.Format("{1}_{0}.xlsx", DateTime.Now.ToString("MM-dd-yyyy"), "DailyOutstanding");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EPPlusReturn ExportDataFromDetailsNewAccounts(List<ReportContent> rContents, Report report, List<string> footerContent, string approvedBy, string UserName)
        {
            try
            {
                EPPlusReturn result = new();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    result.FileByte = _package.WeeklyNewAccountsPackage(rContents, report, package, footerContent, UserName, approvedBy);
                    result.FileName = string.Format("{1}_{0}.xlsx", DateTime.Now.ToString("MM-dd-yyyy"), "NewAccounts");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EPPlusReturn ExportDataFromDetailsAllOutstanding1(List<ReportContent> rContents, Report report, List<string> footerContent, string approvedBy, string UserName)
        {
            try
            {
                EPPlusReturn result = new();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    result.FileByte = _package.MonthlyOutstandingPackage(rContents, report, package, footerContent, UserName, approvedBy);
                    result.FileName = string.Format("{1}_{0}.xlsx", DateTime.Now.ToString("MM-dd-yyyy"), "MonthlyOutstanding(Monetary & Misc)");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EPPlusReturn ExportDataFromDetailsAllOutstanding2(List<ReportContent> rContents, Report report, List<string> footerContent, string approvedBy, string UserName)
        {
            try
            {
                EPPlusReturn result = new();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    result.FileByte = _package.MonthlyOutstandingPackage(rContents, report, package, footerContent, UserName, approvedBy);
                    result.FileName = string.Format("{1}_{0}.xlsx", DateTime.Now.ToString("MM-dd-yyyy"), "MonthlyOutstanding(Non-Monetary)");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ReportsRev> GetReportsRevs(Guid reportsGuid)
        {
            return await _context.ReportsRevs.Where(s => s.ReportsGuid == reportsGuid && s.IsProcessed == false && s.Changes == "Sending").FirstOrDefaultAsync();
        }
        public async Task UpdateReportAndRev(Report report, ReportsRev reportRevs)
        {
            try
            {
                _context.Update(report);
                _context.Update(reportRevs);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task UpdateReport(Report report)
        {
            try
            {
                _context.Update(report);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task UpdateReportRev(ReportsRev reportRevs)
        {
            try
            {
                _context.Update(reportRevs);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SendMakerNotification(string[] contentDetails, string recipient, bool isApproved)
        {
            try
            {
                _emailNotif.SendMakerNotification(contentDetails, recipient, isApproved);
            }
            catch
            {
                throw;
            }
        }
        public void SendApproverNotification(string[] contentDetails, List<string> recipients)
        {
            try
            {
                _emailNotif.SendApproverNotification(contentDetails, recipients);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void SendBranchNotification(string[] contentDetailsBranch, string[] toEmployees, string[] ccEmployees)
        {

            var toRecipients = _context.Users.Where(s => toEmployees.Contains(s.EmployeeId)).Select(s => s.Email).ToList();
            var ccRecipients = _context.Users.Where(s => ccEmployees.Contains(s.EmployeeId)).Select(s => s.Email).ToList();

            _emailNotif.SendBranchNotification(contentDetailsBranch, toRecipients, ccRecipients);
        }
        public async Task<User> GetUserEmployeeID(string LoginName)
        {
            return await _context.Users.SingleOrDefaultAsync(s => s.LoginName == LoginName);
        }
        public string GetHostConfig()
        {
            return Settings.Config.HOST;
        }
        public string GetGetDisplayNameReportCategory(int ReportCategory)
        {
            return ((ReportCategory)ReportCategory).GetDisplayName();
        }
        public async Task<List<string>> GetRecipients(Report query)
        {
            List<string> recipients = new List<string>();
            List<string> employeesIDs = new List<string>();
            List<int> branchIDs = new List<int>();

            string createdBy = query.CreatedBy;

            var UsersBranch = await _context.Users.Include(s => s.BranchAccesses)
                .SingleOrDefaultAsync(s => s.LoginName == createdBy);
            List<BranchAccess> assignedBranches = new();
            if (UsersBranch != null)
            {
                assignedBranches = UsersBranch.BranchAccesses.ToList();
            }
            foreach (var item in assignedBranches)
            {
                employeesIDs.Add(item.EmployeeId);
                branchIDs.Add(item.BranchId);
            }

            var matchingIDs = await _context.BranchAccesses.Where(s => branchIDs.Contains(s.BranchId)).Select(s => s.EmployeeId).ToListAsync();

            var matchingPeople = _context.Users.Where(x => matchingIDs.Contains(x.EmployeeId) && (x.RoleId == 3 || x.RoleId == 5));

            if (await matchingPeople.CountAsync() != 0)
            {
                recipients = await matchingPeople.Select(s => s.Email).ToListAsync();
            }

            return recipients;
        }
    }
}
