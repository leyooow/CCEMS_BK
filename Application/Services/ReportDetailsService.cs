using Application.Contracts.Repositories;
using Application.Models.Helpers;
using Infrastructure.Entities;
using Application.Contracts.Services;
using Application.Models.Responses;
using Application.Models.DTOs.Common;
using Application.Services.Application.Services;
using Application.Models.DTOs.Report;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Collections.Generic;
using Org.BouncyCastle.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace Application.Services
{
    public class ReportDetailsService : IReportDetailsService
    {
        public readonly IReportDetailsRepository _repository;
        private readonly UserClaimsDTO _user;
        private readonly IUserClaimsService _userClaimsService;
        private readonly IMapper _mapper;
        public ReportDetailsService(IReportDetailsRepository repository, IUserClaimsService userClaimsService, IMapper mapper)
        { 
            _repository = repository;
            _mapper = mapper;
            _userClaimsService = userClaimsService;
            _user = _userClaimsService.GetClaims();
        }
        public async Task<PaginatedList<ReportContent>> GetReportContentsList(int id, int page)
        {
            return await _repository.GetReportContentsList(id, page);
        }
        public async Task<ReportDTO> GetReport(int id)
        {
            try
            {
                var report = await _repository.GetReports(id);
                if (!String.IsNullOrEmpty(report.ToRecipients))
                {
                    report.ToList = report.ToRecipients.Split(';').ToList();
                }
                if (!String.IsNullOrEmpty(report.Ccrecipients))
                {
                    report.CCList = report.Ccrecipients.Split(';').ToList();
                }
                return _mapper.Map<ReportDTO>(report);
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<string>> SelectedBranches(int id)
        {
            try
            {
                var report = await _repository.GetReports(id);
                string[] branchNames = await GetBranchNames(report);
                return branchNames.ToList();
            }
            catch
            {
                throw;
            }
        }
        public async Task<string[]> GetBranchNames(Report report)
        {
            string[] branchCodes;

            List<string> br = new List<string>();

            branchCodes = report.SelectedBranches.Split('-');

            foreach (var item in branchCodes)
            {
                string brCode = item.TrimStart('0');
                br.Add(brCode);
            }

            var branchNames = await _repository.GetBranchNames(br);

            return branchNames;
        }
        public async Task<List<DropdownReturn>> PopulateBranchRecipients(string selected = "", string brCode = "")
        {
            string[] strBranchCodes = brCode.Split('-');

            int[] intBranchCodes = Array.ConvertAll(strBranchCodes, s => int.Parse(s));

            List<string> branchEmp = await _repository.GetBranchAccesses(intBranchCodes);

            List<User> usersList = await _repository.GetUsersByBranch(branchEmp);

            List<DropdownReturn> users = new List<DropdownReturn>();

            foreach (var item in usersList)
            {
                DropdownReturn user = new DropdownReturn();
                if (item.MiddleName == null)
                {
                    item.MiddleName = "";
                }
                user.Text = item.EmployeeId;
                user.Value = $"{item.FirstName.ToUpper()} {item.MiddleName.ToUpper()} {item.LastName.ToUpper()} <{item.Email.ToLower()}>";
                if (selected.ToString() == item.EmployeeId)
                    user.IsSelected = true;
                users.Add(user);
            }
            return users;
        }
        public async Task<GenericResponse<EPPlusReturn>> PulloutRequest(int id, string refno)
        {
            string UserName = _user.LoginName ?? string.Empty;

            Report report = await _repository.GetReports(id);

            ReportsRev reportRevs = await _repository.GetReportsRevsLast(report);

            if (report != null)
            {
                //List<ReportContents> rContents = _context.ReportContents.Where(s => s.ReportId == id).ToList(); ;
                List<ReportContent> rContents = await _repository.GetReportContentsList(id, refno);

                //FileInfo file;
                List<string> footerContent = new List<string>();
                EPPlusReturn data = await _repository.GeneratePulloutRequest(UserName, rContents, report, reportRevs, footerContent);
                return ResponseHelper.SuccessResponse<EPPlusReturn>(data);


            }
            return ResponseHelper.ErrorResponse<EPPlusReturn>("");
        }
        public async Task<GenericResponse<EPPlusReturn>> ExportDataFromDetails(int id)
        {
            EPPlusReturn ePPlusReturn = new();
            string UserName = _user.LoginName ?? string.Empty;
            List<string> footerContent = new List<string>();

            Report report = await _repository.GetReports(id);

            ReportsRev reportRevs = await _repository.GetReportsRevsLast(report);

            User maker = await _repository.GetMaker(reportRevs.CreatedBy ?? "");
            User approver = await _repository.GetApprover(reportRevs.ApprovedBy ?? "");

            if (maker != null)
            {
                footerContent.Add(string.Format("{0} {1} {2}", maker.FirstName, maker.MiddleName, maker.LastName));
                footerContent.Add(maker.Role.Description);
            }

            if (approver != null)
            {
                footerContent.Add(string.Format("{0} {1} {2}", approver.FirstName, approver.MiddleName, approver.LastName));
                footerContent.Add(approver.Role.Description);
            }

            if (report != null)
            {
                //var getAging = Settings.Config.IncludeOnlyEscalationReportAgingDays.Split(',');
                //List<ReportContents> rContents = _context.ReportContents.Where(s => s.ReportId == id && getAging.Contains(s.Aging)).ToList(); ;

                List<ReportContent> rContents = await _repository.GetReportContentsList(id);
                foreach (var item in rContents)
                {

                    List<BranchReply> replys = await _repository.GetBranchReplyList(item.ExceptionNo ?? "",item.Id);
                    string holder = "";
                    foreach (var rep in replys)
                    {
                        holder += "," + rep.ActionPlan;
                    }
                    item.ActionPlan = holder;
                }

                if (rContents.Count() == 0)
                {
                    return ResponseHelper.ErrorResponse<EPPlusReturn>("No generated report.");
                }

                rContents = await _repository.GetRiskAssesmentAsync(rContents);

                string approvedBy = await _repository.GetApprovBy(id);

                switch ((ReportCategory)report.ReportCategory)
                {
                    case ReportCategory.DailyExceptionReport:
                        ePPlusReturn = _repository.ExportDataFromDetailsDailyExceptionReport(rContents, report, footerContent, approvedBy, UserName);
                        break;
                    case ReportCategory.RedFlag:
                        ePPlusReturn = _repository.ExportDataFromDetailsRedFlag(rContents, report, footerContent, approvedBy, UserName);
                        break;
                    case ReportCategory.Escalation:
                        ePPlusReturn = await _repository.ExportDataFromDetailsEscalation(rContents, report, footerContent, approvedBy, UserName,id);
                        break;
                    case ReportCategory.NewAccounts:
                        ePPlusReturn = _repository.ExportDataFromDetailsNewAccounts(rContents, report, footerContent, approvedBy, UserName);
                        break;
                    case ReportCategory.AllOutstanding1:
                        ePPlusReturn = _repository.ExportDataFromDetailsAllOutstanding1(rContents, report, footerContent, approvedBy, UserName);
                        break;
                    case ReportCategory.AllOutstanding2:
                        ePPlusReturn = _repository.ExportDataFromDetailsAllOutstanding2(rContents, report, footerContent, approvedBy, UserName);
                        break;
                    default:
                        break;
                }
            }

            return ResponseHelper.SuccessResponse(ePPlusReturn);
        }
        public async Task<GenericResponse<dynamic>> Reject(int id, Guid reportsGuid, string remarks)
        {
            Report report = new Report();
            string loggedUser = _user.LoginName ?? string.Empty;

            try
            {
                if (string.IsNullOrEmpty(remarks))
                {
                    return ResponseHelper.ErrorResponse<dynamic>("Remarks field cannot be empty. Please try again");
                }

                //Todo: Update Report Revs IsProcessed to True
                var reportRevs = await _repository.GetReportsRevs(reportsGuid);
                if (reportRevs != null)
                {
                    bool isRequestor = reportRevs.ModifiedBy == loggedUser ? true : false;

                    if (!isRequestor)
                    {
                        reportRevs.IsProcessed = true;
                        reportRevs.ActionTaken = "Rejected";
                        reportRevs.ApprovedBy = loggedUser;
                        reportRevs.ApprovedDateTime = DateTime.Now;

                        //Todo: Update Main Report status to Sent
                        report = await _repository.GetReports(id);
                        report.Status = (int)ReportStatus.Rejected;

                        await _repository.UpdateReportAndRev(report, reportRevs);
                    }
                    else
                    {
                        return ResponseHelper.ErrorResponse<dynamic>("You cannot approve/reject your own request. Ask for other AOO/BCO's approval");
                    }
                }
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
            }


            var url = $"{_repository.GetHostConfig()}/reports/details/{id}";

            try
            {
                //Send Email Notification to Requestor that request is approved/rejected
                string[] contentDetails = { report.SelectedBranches, report.CreatedBy, "Send Report to Branch", id.ToString(), remarks, url };
                User user = await _repository.GetUserEmployeeID(report.CreatedBy);
                
                //TemporaryBinaryValueGenerator commentout for testing
                //_repository.SendMakerNotification(contentDetails, user.Email, false);
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
            }

            return ResponseHelper.SuccessResponse<dynamic>("Successfully Rejected");
        }
        public async Task<GenericResponse<dynamic>> Approve(int id, Guid reportsGuid)
        {
            Report report = new Report();
            string loggedUser = _user.LoginName ?? string.Empty;

            try
            {
                //Todo: Update Report Revs IsProcessed to True
                var reportRevs = await _repository.GetReportsRevs(reportsGuid);

                if (reportRevs != null)
                {
                    bool isRequestor = reportRevs.ModifiedBy == loggedUser ? true : false;

                    if (!isRequestor)
                    {
                        reportRevs.IsProcessed = true;
                        reportRevs.ActionTaken = "Approved";
                        reportRevs.ApprovedBy = loggedUser;
                        reportRevs.ApprovedDateTime = DateTime.Now;

                        //Todo: Update Main Report status to Sent
                        report = await _repository.GetReports(id);
                        report.Status = (int)ReportStatus.Approved;
                        report.DateSent = DateTime.Now;

                        await _repository.UpdateReportAndRev(report, reportRevs);
                    }
                    else
                    {
                        return ResponseHelper.ErrorResponse<dynamic>("You cannot approve/reject your own request. Ask for other AOO/BCO's approval");
                    }
                }
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
            }


            var url = $"{_repository.GetHostConfig()}/reports/details/{id}";

            try
            {
                //Send Email Notification to Requestor that request is approved/rejected
                string[] contentDetails = { report.SelectedBranches, report.CreatedBy, "Send Report to Branch", id.ToString(), url };
                User user = await _repository.GetUserEmployeeID(report.CreatedBy);

                //temporary comment out

                //_repository.SendMakerNotification(contentDetails, user.Email, true);

                ////Send Email Notification to Branch
                //string[] contentDetailsBranch = { _repository.GetGetDisplayNameReportCategory(report.ReportCategory), report.SelectedBranches, report.Id.ToString(), url };
                //string[] toEmployees = report.ToRecipients.Split(';');
                //string[] ccEmployees = report.Ccrecipients.Split(';');
                //_repository.SendBranchNotification(contentDetailsBranch, toEmployees, ccEmployees);
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
            }

            return ResponseHelper.SuccessResponse<dynamic>("Successfully Approved");
        }
        public async Task<GenericResponse<dynamic>> SendReport(List<string> ToList, List<string> CCList, int id)
        {
            string errMsg = string.Empty;
            var reportToEdit = await _repository.GetReports(id);
            string concatToRecipientsID = string.Empty;
            string concatCCRecipientsID = string.Empty;
            string loggedUser = _user.LoginName ?? string.Empty;

            if (ToList == null)
            {
                return ResponseHelper.ErrorResponse<dynamic>("Please select atleast 1 recipient");
            }

            concatToRecipientsID = String.Join(';', ToList);
            concatCCRecipientsID = CCList == null ? string.Empty : String.Join(';', CCList);

            try
            {
                reportToEdit.CCList = ToList;

                reportToEdit.ToRecipients = concatToRecipientsID;
                reportToEdit.Ccrecipients = concatCCRecipientsID;
                reportToEdit.Status = (int)ReportStatus.PendingApproval;
                await _repository.UpdateReport(reportToEdit);
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
            }

            //Todo: Insert data on Report Revs for Approval of AOO
            if (errMsg == string.Empty)
            {
                try
                {
                    ReportsRev reportRevs = new ReportsRev();

                    reportRevs.FileName = reportToEdit.FileName;
                    reportRevs.Path = reportToEdit.Path;
                    reportRevs.ActionPlan = reportToEdit.ActionPlan;
                    reportRevs.CreatedBy = reportToEdit.CreatedBy;
                    reportRevs.DateGenerated = reportToEdit.DateGenerated;
                    reportRevs.ActionPlanCreated = reportToEdit.ActionPlanCreated;
                    reportRevs.Status = (int)ReportStatus.PendingApproval;
                    reportRevs.ReportCoverage = reportToEdit.ReportCoverage;
                    reportRevs.ReportCategory = reportToEdit.ReportCategory;
                    reportRevs.CoverageDate = reportToEdit.CoverageDate;
                    reportRevs.SelectedBranches = reportToEdit.SelectedBranches;
                    reportRevs.ToRecipients = reportToEdit.ToRecipients;
                    reportRevs.Ccrecipients = reportToEdit.Ccrecipients;
                    reportRevs.Changes = "Sending";
                    reportRevs.IsProcessed = false;
                    reportRevs.ApprovalRemarks = string.Empty;
                    reportRevs.ActionTaken = string.Empty;
                    reportRevs.ReportsGuid = reportToEdit.ReportsGuid;
                    reportRevs.ModifiedBy = loggedUser;
                    reportRevs.ModifiedDateTime = DateTime.Now;

                    await _repository.UpdateReportRev(reportRevs);

                    // Email Notification
                    var url = $"{ _repository.GetHostConfig() }/reports/details/{reportToEdit.Id}";
                    string[] contentDetails = { reportToEdit.SelectedBranches, reportToEdit.CreatedBy, "Send Report to Branch", reportToEdit.Id.ToString(), url };
                    List<string> recipients = await _repository.GetRecipients(reportToEdit);
                    if (recipients.Count != 0)
                        _repository.SendApproverNotification(contentDetails, recipients);
                }
                catch (Exception ex)
                {
                    return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
                }

                if (errMsg != string.Empty) {

                    //Todo: Reverse the changes made to main report
                    reportToEdit.ToRecipients = string.Empty;
                    reportToEdit.Ccrecipients = string.Empty;
                    reportToEdit.Status = (int)ReportStatus.Standby;
                    await _repository.UpdateReport(reportToEdit);
                }
            }

            return ResponseHelper.SuccessResponse<dynamic>("Sending request is now Pending for Approval");
        }
    }
}
