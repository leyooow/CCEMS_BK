using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Report;
using Application.Models.Helpers;
using Application.Models.Responses;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repositories
{
    public class ReportGenerateRepository : IReportGenerateRepository
    {
        private readonly CcemQatContext _context;
        private readonly IReportRepository _reportRepository;
        private readonly string basePath = "C:\\CCEM\\Reports\\"; //parameterize
        public ReportGenerateRepository(CcemQatContext context, IReportRepository reportRepository) 
        {
            _context = context;
            _reportRepository = reportRepository;
        }
        //public async Task<List<DropdownReturn>> PopulateGroupsDropDownList(string EmployeeID)
        //{
        //    try
        //    {
        //        var assignedBranches = _context.BranchAccesses.Where(s => s.EmployeeId == EmployeeID).Select(s => s.BranchId).ToList();
        //        var query = await _context.Groups
        //                    .Where(s => assignedBranches.Contains(Convert.ToInt32(s.Code)))
        //                    .Select(x => new DropdownReturn { Text = x.Name, Value = x.Code })
        //                    .OrderBy(s => s.Text)
        //                    .ToListAsync();


        //        return query;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<DropdownReturn>();
        //    }

        //}

        //public async Task<List<DropdownReturn>> PopulateGroupsDropDownList(string employeeId)
        //{
        //    try
        //    {
        //        // Retrieve the list of branch IDs assigned to the employee
        //        var assignedBranches = await _context.BranchAccesses
        //            .Where(branchAccess => branchAccess.EmployeeId == employeeId)
        //            .Select(branchAccess => branchAccess.BranchId)
        //            .ToListAsync();

        //        // Ensure that s.Code is a valid integer before conversion and filtering
        //        var query = await _context.Groups
        //            .Where(group => int.TryParse(group.Code, out var code) && assignedBranches.Contains(code))
        //            .OrderBy(group => group.Name)
        //            .Select(group => new DropdownReturn
        //            {
        //                Text = group.Name,
        //                Value = group.Code
        //            })
        //            .ToListAsync();

        //        return query;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (if a logging mechanism is in place)
        //        // For example: _logger.LogError(ex, "Error in PopulateGroupsDropDownList");

        //        return new List<DropdownReturn>();
        //    }
        //}

        public async Task<List<DropdownReturn>> PopulateGroupsDropDownList(string employeeId)
        {
            try
            {
                // Retrieve the list of branch IDs assigned to the employee
                var assignedBranches = await _context.BranchAccesses
                    .Where(branchAccess => branchAccess.EmployeeId == employeeId)
                    .Select(branchAccess => branchAccess.BranchId)
                    .ToListAsync();

                // Fetch all groups from the database
                var groups = await _context.Groups.ToListAsync();

                // Filter groups and validate `Code` conversion in-memory
                var filteredGroups = groups
                    .Where(group => int.TryParse(group.Code, out var code) && assignedBranches.Contains(code))
                    .OrderBy(group => group.Name)
                    .Select(group => new DropdownReturn
                    {
                        Text = group.Name,
                        Value = group.Code
                    })
                    .ToList();

                return filteredGroups;
            }
            catch (Exception ex)
            {
                // Log the exception (if applicable)
                // Example: _logger.LogError(ex, "Error in PopulateGroupsDropDownList");

                return new List<DropdownReturn>();
            }
        }


        public async Task<string> InsertReportRecordsToDB(string[] fileDetails, ReportCategory category, ReportCoverage coverage, ReportStatus status,
            List<ExceptionItem> exception, List<ExceptionItemRev> exceptionRevs, List<ReportContent> rContents, string loggedRole, string Username)
        {
            string errMsg = string.Empty;
            var report = new Report();
            var reportRevs = new ReportsRev();

            try
            {
                if (exception != null || exceptionRevs != null)
                {
                    Guid secondaryID = Guid.NewGuid();

                    report.Path = fileDetails[0];
                    report.FileName = fileDetails[1];
                    report.CoverageDate = Convert.ToDateTime(fileDetails[2]);
                    report.SelectedBranches = fileDetails[3];
                    report.ActionPlan = string.Empty;
                    report.DateGenerated = DateTime.Now;
                    if (loggedRole.Equals("AOO"))
                    {
                        if (Settings.Config.AOOGenerateReportWithoutApproval.Equals("1"))
                        {
                            report.Status = (int)ReportStatus.Approved;
                        }
                        else
                        {
                            report.Status = (int)status;
                        }
                    }
                    else
                    {
                        report.Status = (int)status;
                    }
                    report.ReportCategory = (int)category;
                    report.ReportCoverage = (int)coverage;
                    report.CreatedBy = Username;
                    report.ReportsGuid = secondaryID;
                    report.ReportContents = rContents;
                    report.ApprovalRemarks = "";

                    reportRevs.Path = fileDetails[0];
                    reportRevs.FileName = fileDetails[1];
                    reportRevs.CoverageDate = Convert.ToDateTime(fileDetails[2]);
                    reportRevs.SelectedBranches = fileDetails[3];
                    reportRevs.ActionTaken = fileDetails[4];
                    reportRevs.Changes = fileDetails[5];
                    reportRevs.IsProcessed = Convert.ToBoolean(fileDetails[6]);
                    reportRevs.ActionPlan = string.Empty;
                    reportRevs.DateGenerated = DateTime.Now;
                    if (loggedRole.Equals("AOO"))
                    {
                        if (Settings.Config.AOOGenerateReportWithoutApproval.Equals("1"))
                        {
                            reportRevs.Status = (int)ReportStatus.Approved;
                        }
                        else
                        {
                            reportRevs.Status = (int)status;
                        }
                    }
                    else
                    {
                        reportRevs.Status = (int)status;
                    }
                    reportRevs.ReportCategory = (int)category;
                    reportRevs.ReportCoverage = (int)coverage;
                    reportRevs.ApprovalRemarks = string.Empty;
                    reportRevs.CreatedBy = Username;
                    reportRevs.ReportsGuid = secondaryID;
                    reportRevs.ApprovalRemarks = "";

                }

                _context.Add(report);
                _context.Add(reportRevs);
                //_context.AddRange(rContents);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }
        public async Task<List<ReportContent>> RedFlagDaily(List<ExceptionItem> list, DateTime? cutOffDate)
        {
            List<ReportContent> rfList = new List<ReportContent>();

            foreach (var item in list)
            {
                var subExceptions = item.ExceptionCodes;
                var actionPlan = item.ActionPlans;
                if (actionPlan == null)
                    actionPlan = new List<ActionPlan>();
                foreach (var subEx in subExceptions)
                {

                    Deviation deviationLookup = await _reportRepository.GetDeviations(subEx.ExCode);
                    string[] account = _reportRepository.GetAvailableAccount(item);
                    var agingResult = await _reportRepository.ComputeAging(item.TransactionDate, cutOffDate, subEx.SubReferenceNo, (DeviationStatus)subEx.DeviationStatus);

                    ReportContent rf = new ReportContent();
                    rf.Id = Guid.NewGuid();
                    rf.ExceptionNo = subEx.SubReferenceNo;
                    rf.BranchCode = item.BranchCode;
                    rf.BranchName = item.BranchName;
                    rf.Area = item.Area;
                    rf.Division = item.Division;
                    rf.TransactionDate = item.TransactionDate.ToString("MM/dd/yyyy");
                    rf.Aging = agingResult.ToString();
                    rf.AgingCategory = _reportRepository.GetAgingCategory(Convert.ToDouble(rf.Aging)).ToString();
                    rf.Process = _reportRepository.GetProcessForNonRevs(item);
                    rf.AccountNo = account[0];
                    rf.AccountName = account[1];
                    rf.Deviation = deviationLookup.Deviation1;
                    rf.RiskClassification = deviationLookup.RiskClassification;
                    rf.DeviationCategory = deviationLookup.Category;
                    rf.Amount = _reportRepository.GetAvailableAmount(item);
                    rf.PersonResponsible = item.PersonResponsible;
                    rf.OtherPersonResponsible = item.OtherPersonResponsible;
                    rf.Remarks = item.Remarks;
                    rf.ActionPlan = actionPlan.Count() == 0 ? "" : actionPlan.FirstOrDefault().ActionPlan1;
                    rf.EncodedBy = item.CreatedBy;
                    rf.RootCause = item.RootCause.GetDisplayName();
                    rf.DeviationApprover = item.DeviationApprover;

                    rfList.Add(rf);
                }
            }

            return rfList;
        }
        public async Task<List<ReportContent>> OutstandingDaily(List<ExceptionItem> list, DateTime? cutOffDate)
        {
            List<ReportContent> osList = new List<ReportContent>();

            foreach (var item in list)
            {
                var subExceptions = item.ExceptionCodes;
                var actionPlan = item.ActionPlans;
                if (actionPlan == null)
                    actionPlan = new List<ActionPlan>();

                foreach (var subEx in subExceptions)
                {
                    if ((DeviationStatus)subEx.DeviationStatus != DeviationStatus.Outstanding)
                    {
                        continue;
                    }

                    Deviation deviationLookup = await _reportRepository.GetDeviations(subEx.ExCode);
                    string[] account = _reportRepository.GetAvailableAccount(item);
                    var agingResult = await _reportRepository.ComputeAging(item.TransactionDate, cutOffDate, subEx.SubReferenceNo, (DeviationStatus)subEx.DeviationStatus);

                    ReportContent os = new ReportContent();
                    os.Id = Guid.NewGuid();
                    os.ExceptionNo = subEx.SubReferenceNo;
                    os.BranchCode = item.BranchCode;
                    os.BranchName = item.BranchName;
                    os.Area = item.Area;
                    os.Division = item.Division;
                    os.TransactionDate = item.TransactionDate.ToString("MM/dd/yyyy");
                    os.Aging = agingResult.ToString();
                    os.AgingCategory = _reportRepository.GetAgingCategory(Convert.ToDouble(os.Aging)).ToString();
                    os.Process = _reportRepository.GetProcessForNonRevs(item);
                    os.AccountNo = account[0];
                    os.AccountName = account[1];
                    os.Deviation = deviationLookup.Deviation1;
                    os.RiskClassification = deviationLookup.RiskClassification;
                    os.DeviationCategory = deviationLookup.Category;
                    os.Amount = _reportRepository.GetAvailableAmount(item);
                    os.PersonResponsible = item.PersonResponsible;
                    os.OtherPersonResponsible = item.OtherPersonResponsible;
                    os.Remarks = item.Remarks;
                    os.ActionPlan = actionPlan.Count() == 0 ? "" : actionPlan.FirstOrDefault().ActionPlan1;
                    os.EncodedBy = item.CreatedBy;
                    os.RootCause = item.RootCause.GetDisplayName();
                    os.DeviationApprover = item.DeviationApprover;

                    osList.Add(os);
                }
            }

            return osList;
        }
        public async Task<List<ReportContent>> NewAccountsWeekly(List<ExceptionItem> list, DateTime? cutOffDate)
        {
            List<ReportContent> naList = new List<ReportContent>();

            foreach (var item in list)
            {
                var subExceptions = item.ExceptionCodes;
                var actionPlan = item.ActionPlans;
                if (actionPlan == null)
                    actionPlan = new List<ActionPlan>();

                foreach (var subEx in subExceptions)
                {
                    if ((DeviationStatus)subEx.DeviationStatus != DeviationStatus.Outstanding)
                    {
                        continue;
                    }

                    Deviation deviationLookup = await _reportRepository.GetDeviations(subEx.ExCode);
                    string[] account = _reportRepository.GetAvailableAccount(item);
                    var agingResult = await _reportRepository.ComputeAging(item.TransactionDate, cutOffDate, subEx.SubReferenceNo, (DeviationStatus)subEx.DeviationStatus);

                    ReportContent na = new ReportContent();
                    na.Id = Guid.NewGuid();
                    //na.ExceptionNo value changed to Sub-ERN from Main ERN
                    na.ExceptionNo = subEx.SubReferenceNo;
                    na.BranchCode = item.BranchCode;
                    na.BranchName = item.BranchName;
                    na.Area = item.Area;
                    na.Division = item.Division;
                    na.TransactionDate = item.TransactionDate.ToString("MM/dd/yyyy");
                    na.Aging = agingResult.ToString();
                    na.AgingCategory =  _reportRepository.GetAgingCategory(Convert.ToDouble(na.Aging)).ToString();
                    na.Process =  _reportRepository.GetProcessForNonRevs(item);
                    na.AccountNo = account[0];
                    na.AccountName = account[1];
                    na.Deviation = deviationLookup.Deviation1;
                    na.RiskClassification = deviationLookup.RiskClassification;
                    na.DeviationCategory = deviationLookup.Category;
                    na.Amount = _reportRepository.GetAvailableAmount(item);
                    na.PersonResponsible = item.PersonResponsible;
                    na.OtherPersonResponsible = item.OtherPersonResponsible;
                    na.Remarks = item.Remarks;
                    na.ActionPlan = actionPlan.Count == 0 ? "" : actionPlan.FirstOrDefault().ActionPlan1;
                    na.EncodedBy = item.CreatedBy;
                    na.RootCause = item.RootCause.GetDisplayName();
                    na.DeviationApprover = item.DeviationApprover;

                    naList.Add(na);
                }
            }

            return naList;
        }
        public async Task<List<ReportContent>> EscalationWeekly(List<ExceptionItem> list, DateTime? cutOffDate)
        {
            List<ReportContent> escalationList = new List<ReportContent>();

            foreach (var item in list)
            {
                var subExceptions = item.ExceptionCodes;
                var actionPlan = item.ActionPlans;
                if (actionPlan == null)
                    actionPlan = new List<ActionPlan>();

                foreach (var subEx in subExceptions)
                {
                    if ((DeviationStatus)subEx.DeviationStatus != DeviationStatus.Outstanding)
                    {
                        continue;
                    }

                    Deviation deviationLookup = await _reportRepository.GetDeviations(subEx.ExCode);
                    string[] account = _reportRepository.GetAvailableAccount(item);
                    var agingResult = await _reportRepository.ComputeAging(item.TransactionDate, cutOffDate, subEx.SubReferenceNo, (DeviationStatus)subEx.DeviationStatus);

                    ReportContent na = new ReportContent();
                    na.Id = Guid.NewGuid();
                    na.ExceptionNo = subEx.SubReferenceNo;
                    na.BranchCode = item.BranchCode;
                    na.BranchName = item.BranchName;
                    na.Area = item.Area;
                    na.Division = item.Division;
                    na.TransactionDate = item.TransactionDate.ToString("MM/dd/yyyy");
                    na.Aging = agingResult.ToString();
                    na.AgingCategory = _reportRepository.GetAgingCategory(Convert.ToDouble(na.Aging)).ToString();
                    na.Process =  _reportRepository.GetProcessForNonRevs(item);
                    na.AccountNo = account[0];
                    na.AccountName = account[1];
                    na.Deviation = deviationLookup.Deviation1;
                    na.RiskClassification = deviationLookup.RiskClassification;
                    na.DeviationCategory = deviationLookup.Category;
                    na.Amount = _reportRepository.GetAvailableAmount(item);
                    na.PersonResponsible = item.PersonResponsible;
                    na.OtherPersonResponsible = item.OtherPersonResponsible;
                    na.Remarks = item.Remarks;
                    na.ActionPlan = actionPlan.Count == 0 ? "" : actionPlan.FirstOrDefault().ActionPlan1;
                    na.EncodedBy = item.CreatedBy;
                    na.RootCause = item.RootCause.GetDisplayName();
                    na.DeviationApprover = item.DeviationApprover;

                    escalationList.Add(na);
                }
            }

            return escalationList;
        }
        public async Task<List<ReportContent>> AllOutstandingMonthly(List<ExceptionItem> list, DateTime? cutOffDate)
        {
            List<ReportContent> osList = new List<ReportContent>();

            foreach (var item in list)
            {
                var subExceptions = item.ExceptionCodes;
                var actionPlan = item.ActionPlans;
                if (actionPlan == null)
                    actionPlan = new List<ActionPlan>();

                foreach (var subEx in subExceptions)
                {
                    if ((DeviationStatus)subEx.DeviationStatus != DeviationStatus.Outstanding)
                    {
                        continue;
                    }

                    Deviation deviationLookup = await _reportRepository.GetDeviations(subEx.ExCode);
                    string[] account = _reportRepository.GetAvailableAccount(item);
                    var agingResult = await _reportRepository.ComputeAging(item.TransactionDate, cutOffDate, subEx.SubReferenceNo, (DeviationStatus)subEx.DeviationStatus);

                    ReportContent os = new ReportContent();
                    os.Id = Guid.NewGuid();
                    os.ExceptionNo = subEx.SubReferenceNo;
                    os.BranchCode = item.BranchCode;
                    os.BranchName = item.BranchName;
                    os.Area = item.Area;
                    os.Division = item.Division;
                    os.TransactionDate = item.TransactionDate.ToString("MM/dd/yyyy");
                    os.Aging = agingResult.ToString();
                    os.AgingCategory = _reportRepository.GetAgingCategory(Convert.ToDouble(os.Aging)).ToString();
                    os.Process = _reportRepository.GetProcessForNonRevs(item);
                    os.AccountNo = account[0];
                    os.AccountName = account[1];
                    os.Deviation = deviationLookup.Deviation1;
                    os.RiskClassification = deviationLookup.RiskClassification;
                    os.DeviationCategory = deviationLookup.Category;
                    os.Amount = _reportRepository.GetAvailableAmount(item);
                    os.PersonResponsible = item.PersonResponsible;
                    os.OtherPersonResponsible = item.OtherPersonResponsible;
                    os.Remarks = item.Remarks;
                    os.ActionPlan = actionPlan.Count() == 0 ? "" : actionPlan.FirstOrDefault().ActionPlan1;
                    os.EncodedBy = item.CreatedBy;
                    os.RootCause = item.RootCause.GetDisplayName();
                    os.DeviationApprover = item.DeviationApprover;

                    osList.Add(os);
                }
            }

            return osList;
        }
        public async Task<List<ExceptionItem>> GetProcessDailyOutstanding(DateTime dateCoverage)
        {

            return await _context.ExceptionItems
                        .Include(s => s.Monetaries)
                        .Include(s => s.Miscs)
                        .Include(s => s.ExceptionCodes)
                        //.Include(s => s.ActionPlan)
                        .Where(s => s.TransactionDate == dateCoverage.Date
                        && (MainStatus)s.Status == MainStatus.Open
                        && ((TransactionTypeEnum)s.Type == TransactionTypeEnum.Monetary
                        || (TransactionTypeEnum)s.Type == TransactionTypeEnum.Miscellaneous))
                        .ToListAsync();
        }
        public async Task<List<ExceptionItem>> GetProcessDailyRedFlag(DateTime dateCoverage)
        {

            return await _context.ExceptionItems.Include(s => s.Monetaries)
                .Include(s => s.NonMonetaries).Include(s => s.Miscs).Include(s => s.ExceptionCodes)//.Include(s => s.ActionPlan)
                .Where(s => s.RedFlag == true && s.TransactionDate.Date == dateCoverage.Date).ToListAsync();
        }
        public async Task<List<ExceptionItem>> GetProcessWeeklyEscalation(DateTime dateFrom, DateTime dateTo)
        {

            return await _context.ExceptionItems.Include(s => s.Monetaries).Include(s => s.NonMonetaries).Include(s => s.Miscs).Include(s => s.ExceptionCodes)//.Include(s => s.ActionPlan)
                        .Where(s => ((TransactionTypeEnum)s.Type == TransactionTypeEnum.Monetary || (TransactionTypeEnum)s.Type == TransactionTypeEnum.Miscellaneous || (TransactionTypeEnum)s.Type == TransactionTypeEnum.NonMonetary) &&
                        (MainStatus)s.Status == MainStatus.Open && s.TransactionDate >= dateFrom.Date && s.TransactionDate <= dateTo.Date).ToListAsync();
        }
        public async Task<List<ExceptionItem>> GetProcessAllOutstandingMonetaryAndMiscellaneous(DateTime dateFrom, DateTime dateTo)
        {

            return await _context.ExceptionItems.Include(s => s.Monetaries).Include(s => s.Miscs).Include(s => s.ExceptionCodes)//.Include(s => s.ActionPlan)
                        .Where(s => (TransactionTypeEnum)s.Type != TransactionTypeEnum.NonMonetary && (s.TransactionDate.Date >= dateFrom.Date && s.TransactionDate.Date <= dateTo.Date))
                        .ToListAsync();
        }
        public async Task<List<ExceptionItem>> GetProcessAllOutstandingNonMonetary(DateTime dateFrom, DateTime dateTo)
        {

            return await _context.ExceptionItems.Include(s => s.NonMonetaries).Include(s => s.ExceptionCodes)//.Include(s => s.ActionPlan)
                        .Where(s => (TransactionTypeEnum)s.Type == TransactionTypeEnum.NonMonetary && (MainStatus)s.Status == MainStatus.Open && (s.TransactionDate.Date >= dateFrom.Date && s.TransactionDate.Date <= dateTo.Date))
                        .ToListAsync();
        }
        public async Task<List<ExceptionItem>> GetProcessWeeklyNewAccounts(DateTime dateFrom, DateTime dateTo)
        {

            return await _context.ExceptionItems.Include(s => s.NonMonetaries).Include(s => s.ExceptionCodes)//.Include(s => s.ActionPlan)
                            .Where(s => (TransactionTypeEnum)s.Type == TransactionTypeEnum.NonMonetary && s.TransactionDate >= dateFrom.Date && s.TransactionDate <= dateTo.Date)
                        .ToListAsync();
        }
    }
}
