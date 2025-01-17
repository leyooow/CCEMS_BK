using Application.Contracts.Repositories;
using Application.Models.DTOs.Report;
using Application.Models.Helpers;
using Application.Models.Responses;
using Domain.FEntities;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly CcemQatContext _context;
        private readonly SitcbsContext _sibsDA;
        private readonly EPPlusPackages _package;
        public ReportRepository(CcemQatContext context, SitcbsContext sibsDA, EPPlusPackages package)
        {
            _context = context;
            _sibsDA = sibsDA;
            _package = package;
        }
        public IQueryable<Report> GetEmptyList()
        {
            try
            {
                IQueryable<Report> query = _context.Reports
                    .Where(s => 1 != 1);
                return query;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IQueryable<Report> GetList(string searchString, List<string> employeesAccess)
        {
            try
            {
                IQueryable<Report> query = _context.Reports
                    .Where(s => employeesAccess.Contains(s.CreatedBy))
                    .OrderByDescending(s => s.DateGenerated);

                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();

                    query = query.Where(s =>
                        s.SelectedBranches.ToString().ToLower().Contains(searchString) ||
                        s.CreatedBy.ToString().ToLower().Contains(searchString))
                        .OrderByDescending(s => s.DateGenerated);
                }
                return query;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IQueryable<Report> GetListFilterWithPendingAndApproved(string searchString, List<string> employeesAccess)
        {
            try
            {
                IQueryable<Report> query = _context.Reports
                    .Where(s => employeesAccess.Contains(s.CreatedBy) &&
                                ((ReportStatus)s.Status == ReportStatus.PendingApproval || (ReportStatus)s.Status == ReportStatus.Approved))
                    .OrderByDescending(s => s.DateGenerated);

                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();

                    query = query.Where(s =>
                        s.SelectedBranches.ToString().ToLower().Contains(searchString) ||
                        s.CreatedBy.ToString().ToLower().Contains(searchString))
                        .OrderByDescending(s => s.DateGenerated);
                }
                return query;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IQueryable<Report> GetListFilterWithApproved(string searchString,List<string> employeesAccess)
        {
            try
            {
                IQueryable<Report> query =  _context.Reports
                           .Where(s => employeesAccess.Contains(s.CreatedBy) && (ReportStatus)s.Status == ReportStatus.Approved)
                           .OrderByDescending(s => s.DateGenerated);

                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();

                    query = query.Where(s =>
                        s.SelectedBranches.ToString().ToLower().Contains(searchString) ||
                        s.CreatedBy.ToString().ToLower().Contains(searchString))
                        .OrderByDescending(s => s.DateGenerated);
                }
                return query;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<Group>> PopulateGroupsDropDownList(string empID)
        {
            try
            {
                var assignedBranches = _context.BranchAccesses.Where(s => s.EmployeeId == empID).Select(s => s.BranchId).ToList();
                var query = _context.Groups.Where(s => assignedBranches.Contains(Convert.ToInt32(s.Code))).OrderBy(s => s.Name);
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<GenericResponse<EPPlusReturn>> GeneratePervasivenessLogic(DownloadAdhocViewModel vm)
        {
            try
            {
                #region Pervasiveness Logic
                EPPlusReturn result = new();

                var pervasive = await _context.ExceptionItems
                    .Include(s => s.ExceptionCodes)
                    .Where(s => s.EmployeeId == vm.PR.EmployeeID
                        && s.DateCreated >= vm.DateFrom.Date
                        && s.DateCreated <= vm.DateTo.Date)
                    .ToListAsync();

                List<PervasivenessFormat> pervasiveList = new List<PervasivenessFormat>();
                List<PervasivenessFormat> pervasiveList2 = new List<PervasivenessFormat>();

                foreach (var item in pervasive)
                {
                    var subExceptions = item.ExceptionCodes;

                    foreach (var subEx in subExceptions)
                    {
                        PervasivenessFormat pervasiveFormat = new PervasivenessFormat();

                        pervasiveFormat.EmpID = item.EmployeeId;
                        pervasiveFormat.EmpName = item.PersonResponsible + ", " + item.OtherPersonResponsible;
                        pervasiveFormat.BranchCode = item.BranchCode;
                        pervasiveFormat.ExceptionNo = item.RefNo.ToString();
                        pervasiveFormat.SubExceptionNo = subEx.SubReferenceNo.ToString();
                        pervasiveFormat.Deviation = await this.GetDeviationDesc(subEx.ExCode);
                        pervasiveFormat.DeviationCategory = await this.GetDeviationCategoryDesc(subEx.ExCode);
                        pervasiveFormat.DateRecorded = item.TransactionDate.ToString("yyyy/MM/dd");
                        pervasiveFormat.TimesComitted = "";
                        pervasiveFormat.Status = (DeviationStatus)subEx.DeviationStatus;
                        pervasiveList.Add(pervasiveFormat);
                    }
                }

                pervasiveList = pervasiveList.OrderBy(s => s.DeviationCategory).ToList();

                string prev = string.Empty;
                int ave = 0;
                foreach (var item in pervasiveList)
                {
                    PervasivenessFormat pervasiveFormat = new PervasivenessFormat();

                    if (prev != item.DeviationCategory)
                    {
                        pervasiveFormat.EmpID = item.EmpID;
                        pervasiveFormat.EmpName = item.EmpName;
                        pervasiveFormat.BranchCode = item.BranchCode;
                        pervasiveFormat.ExceptionNo = item.ExceptionNo;
                        pervasiveFormat.SubExceptionNo = item.SubExceptionNo;
                        pervasiveFormat.Deviation = item.Deviation;
                        pervasiveFormat.DeviationCategory = item.DeviationCategory;
                        pervasiveFormat.TimesComitted = pervasiveList.Where(s => s.ExceptionNo == item.ExceptionNo).Count().ToString();
                        pervasiveFormat.DateRecorded = item.DateRecorded;
                        pervasiveFormat.Status = item.Status;
                        pervasiveList2.Add(pervasiveFormat);

                        if (Convert.ToInt32(pervasiveFormat.TimesComitted) > 1)
                        {
                            ave += 1;
                        }
                    }
                    else
                    {
                        pervasiveFormat.EmpID = item.EmpID;
                        pervasiveFormat.EmpName = item.EmpName;
                        pervasiveFormat.BranchCode = item.BranchCode;
                        pervasiveFormat.ExceptionNo = item.ExceptionNo;
                        pervasiveFormat.SubExceptionNo = item.SubExceptionNo;
                        pervasiveFormat.Deviation = item.Deviation;
                        pervasiveFormat.DeviationCategory = item.DeviationCategory;
                        pervasiveFormat.TimesComitted = pervasiveList.Where(s => s.ExceptionNo == item.ExceptionNo).Count().ToString();
                        pervasiveFormat.DateRecorded = item.DateRecorded;
                        pervasiveFormat.Status = item.Status;
                        pervasiveList2.Add(pervasiveFormat);
                    }

                    prev = item.DeviationCategory;
                }

                if (pervasiveList2.Count > 0)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        result.FileByte = await Task.Run(() =>
                            _package.GeneratePervasivenessReport(pervasiveList2, package, ave, vm.DateFrom.Date, vm.DateTo.Date));
                        result.FileName = $"Pervasiveness[{vm.PR.EmployeeID}].xlsx";
                        return ResponseHelper.SuccessResponse<EPPlusReturn>(result, "");
                    }
                }
                return ResponseHelper.ErrorResponse<EPPlusReturn>("No extracted data from the filtered query");
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<GenericResponse<EPPlusReturn>> GenerateRegularizationTATLogic(DownloadAdhocViewModel vm)
        {
            try
            {
                #region Regularization TAT Logic
                EPPlusReturn result = new();

                var regTAT = await _context.ExceptionItems
                    .Include(s => s.ExceptionCodes)
                    .Where(s => s.EmployeeId == vm.RT.EmployeeID
                        && s.TransactionDate >= vm.DateFrom.Date
                        && s.TransactionDate <= vm.DateTo.Date)
                    .ToListAsync();

                List<RegularizationTATFormat> rTatList = new();

                foreach (var item in regTAT)
                {
                    var subExceptions = item.ExceptionCodes;

                    foreach (var subEx in subExceptions)
                    {

                        var latestChanges = await _context.ExceptionCodeRevs
                            .Where(s => s.SubReferenceNo == subEx.SubReferenceNo
                                && (DeviationStatus)s.DeviationStatus != DeviationStatus.Outstanding
                                && s.ActionTaken == "Approved"
                                && s.TaggingDate >= vm.DateFrom.Date
                                && s.TaggingDate <= vm.DateTo.Date).OrderBy(s => s.Id).LastOrDefaultAsync();

                        if (latestChanges == null)
                        {
                            latestChanges = await _context.ExceptionCodeRevs
                                .Where(s => s.SubReferenceNo == subEx.SubReferenceNo
                                    && s.Changes == "Migrated"
                                    && s.TaggingDate >= vm.DateFrom.Date
                                    && s.TaggingDate <= vm.DateTo.Date).OrderBy(s => s.Id).LastOrDefaultAsync() ?? new();
                        }
                        RegularizationTATFormat rTatFormat = new RegularizationTATFormat
                        {
                            PersonResponsibleID = item.EmployeeId,
                            PersonResponsibleName = item.PersonResponsible + ", " + item.OtherPersonResponsible,
                            //rTatFormat.OtherPersonResponsibleName = item.OtherPersonResponsible;
                            BranchCode = item.BranchCode,
                            BranchName = item.BranchName,
                            ExceptionNo = item.RefNo.ToString(),
                            SubExceptionNo = subEx.SubReferenceNo.ToString(),
                            Deviation = await this.GetDeviationDesc(subEx.ExCode),
                            DeviationCategory = await this.GetDeviationCategoryDesc(subEx.ExCode),
                            TransactionDate = item.TransactionDate.ToString("MM/dd/yyyy"),
                            RegularizeDate = latestChanges == null ? "" : latestChanges.TaggingDate.ToString("MM/dd/yyyy"),
                            Status = latestChanges == null ? DeviationStatus.Outstanding : (DeviationStatus)subEx.DeviationStatus
                        };

                        if (latestChanges != null)
                        {
                            if (latestChanges.TaggingDate != DateTime.MinValue)
                            {
                                rTatFormat.RegularizeDate = latestChanges.TaggingDate.ToString("MM/dd/yyyy");
                                rTatFormat.DaysOutstanding = await this.CalculateOutstandingDays(item.TransactionDate, latestChanges.TaggingDate, vm.DateTo.Date);
                            }
                            else
                            {
                                rTatFormat.RegularizeDate = "";
                                rTatFormat.DaysOutstanding = await this.CalculateOutstandingDays(item.TransactionDate, latestChanges.ApprovedDateTime, vm.DateTo.Date);
                            }
                        }
                        else
                        {
                            rTatFormat.RegularizeDate = "";
                            rTatFormat.DaysOutstanding = await this.CalculateOutstandingDays(item.TransactionDate, (DateTime?)null, vm.DateTo.Date);
                        }

                        rTatList.Add(rTatFormat);
                    }
                }

                if (rTatList.Count > 0)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        result.FileByte = await Task.Run(() =>
                            _package.GenerateRegularizationTATReport(rTatList, package, vm.DateFrom, vm.DateTo));
                        result.FileName = $"RegularizationTAT[{DateTime.Now:yyyy_MM_dd}].xlsx";
                        return ResponseHelper.SuccessResponse<EPPlusReturn>(result, "");
                    }
                }
                return ResponseHelper.ErrorResponse<EPPlusReturn>("No extracted data from the filtered query");
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<GenericResponse<EPPlusReturn>> GenerateExceptionAdhocsLogic(DownloadAdhocViewModel vm, string empID)
        {
            try
            {

                #region Exception Adhocs Logic
                EPPlusReturn result = new();

                AdhocStatus status = vm.EA.ExceptionStatus;

                List<ExceptionItem> exception = new List<ExceptionItem>();
                List<ExceptionItemRev> exceptionRevs = new List<ExceptionItemRev>();
                List<AdhocsOthers> adhocOthers = new List<AdhocsOthers>();
                List<AdhocsAllDeleted> adhocAllDeletedList = new List<AdhocsAllDeleted>();

                var branch = await _context.BranchAccesses
                    .Where(s => s.EmployeeId == empID)
                    .Select(s => s.BranchId.ToString())
                    .ToListAsync();

                if (status != AdhocStatus.Deleted)
                {
                    exception = _context.ExceptionItems
                        .Include(s => s.ExceptionCodes)
                        //.Include(s => s.ActionPlans) Jawopogi
                        .Include(s => s.Monetaries)
                        .Include(s => s.NonMonetaries)
                        .Include(s => s.Miscs)
                        .Where(s => branch.Contains(s.BranchCode) && (s.TransactionDate.Date >= vm.DateFrom.Date && s.TransactionDate.Date <= vm.DateTo.Date)).ToList();

                    adhocOthers = await AdhocsOthers(exception, status, vm.DateFrom.Date, vm.DateTo.Date);

                    if (adhocOthers.Count > 0)
                    {
                        string title = string.Empty;

                        if (status == AdhocStatus.Regularized)
                        {
                            title = "All Regularized Exceptions";

                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage package = new ExcelPackage())
                            {
                                result.FileByte = _package.GenerateRegularizeAdhoc(adhocOthers, package, status, vm.DateFrom.Date, vm.DateTo.Date);
                                result.FileName = string.Format("{1}_{0}.xlsx", DateTime.Now.ToString("MM-dd-yyyy"), title);
                                return ResponseHelper.SuccessResponse<EPPlusReturn>(result, "");
                            }
                        }
                        if (status == AdhocStatus.ForCompliance)
                        {
                            title = "Exceptions For Compliance";

                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage package = new ExcelPackage())
                            {
                                result.FileByte = _package.GenerateForComplianceAdhoc(adhocOthers, package, status, vm.DateFrom.Date, vm.DateTo.Date);
                                result.FileName = string.Format("{1}_{0}.xlsx", DateTime.Now.ToString("MM-dd-yyyy"), title);
                                return ResponseHelper.SuccessResponse<EPPlusReturn>(result, "");
                            }
                        }
                        if (status == AdhocStatus.Dispensed)
                        {
                            title = "Exceptions For Dispensed";

                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage package = new ExcelPackage())
                            {
                                result.FileByte = _package.GenerateForDispensedAdhoc(adhocOthers, package, status, vm.DateFrom.Date, vm.DateTo.Date);
                                result.FileName = string.Format("{1}_{0}.xlsx", DateTime.Now.ToString("MM-dd-yyyy"), title);
                                return ResponseHelper.SuccessResponse<EPPlusReturn>(result, "");
                            }
                        }
                    }
                    else
                    {
                        return ResponseHelper.ErrorResponse<EPPlusReturn>("No extracted data from the filtered query");
                    }
                }

                if (status == AdhocStatus.Deleted)
                {
                    exceptionRevs = _context.ExceptionItemRevs
                        .Include(s => s.ExceptionCodeRevs)
                        //.Include(s => s.ActionPlans) Jawopogi
                        .Include(s => s.MonetaryRevs)
                        .Include(s => s.NonMonetaryRevs)
                        .Include(s => s.MiscRevs)
                        .Where(s => s.Changes == "Delete" && s.IsProcessed && branch.Contains(s.BranchCode) &&
                        (s.TransactionDate.Date >= vm.DateFrom.Date && s.TransactionDate.Date <= vm.DateTo.Date)).ToList();

                    adhocAllDeletedList = await AllDeletedAdhocs(exceptionRevs, vm.DateFrom.Date, vm.DateTo.Date, status);

                    if (adhocAllDeletedList.Count > 0)
                    {
                        string title = string.Empty;

                        if (status == AdhocStatus.Deleted)
                        {
                            title = "All Deleted Exceptions";
                        }

                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (ExcelPackage package = new ExcelPackage())
                        {
                            result.FileByte = _package.GenerateAllDeletedAdhoc(adhocAllDeletedList, package, status, vm.DateFrom.Date, vm.DateTo.Date);
                            result.FileName = string.Format("{1}_{0}.xlsx", DateTime.Now.ToString("MM-dd-yyyy"), title);
                            return ResponseHelper.SuccessResponse<EPPlusReturn>(result, "");
                        }
                    }
                    else
                    {
                        return ResponseHelper.ErrorResponse<EPPlusReturn>("No extracted data from the filtered query");
                    }
                }
                return ResponseHelper.ErrorResponse<EPPlusReturn>("No extracted data from the filtered query");
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region Get Values
        public async Task<string> GetDeviationDesc(int Id)
        {
            var deviation = await (from s in _context.Deviations
                                where s.Id == Id
                                select s).FirstOrDefaultAsync();
            if (deviation != null)
                return deviation.Deviation1;
            return "";
        }
        public async Task<string> GetDeviationCategoryDesc(int Id)
        {
            var deviationCategory = await (from s in _context.Deviations
                                        where s.Id == Id
                                        select s).FirstOrDefaultAsync();
            if (deviationCategory != null)
                return deviationCategory.Category;
            return "";
        }
        public async Task<double> CalculateOutstandingDays(DateTime tranDate, DateTime? dateModified, DateTime cutOffDate)
        {
            double age = 0;
            DateTime logicDate = cutOffDate; //DateTime.Now;

            if (dateModified != null)
            {
                if (dateModified < cutOffDate)
                {
                    logicDate = (DateTime)dateModified;
                }
            }

            age = (logicDate - tranDate).TotalDays;
            age = Math.Round(age);

            //Get Non Business Days
            DataTable dt = await this.GetBankNonBusinessDays(tranDate, logicDate);
            int nbdCount = dt.Rows.Count;
            age = (age - nbdCount);

            return age;
        }
        public async Task<List<string>> GetEmeployeesWithAccess(string empID)
        {

            List<string> employeesAccess;
            List<string> empIDs = new List<string>();
            List<User> usr = new List<User>();

            var branch = await _context.BranchAccesses
                .Where(s => s.EmployeeId == empID)
                .Select(s => s.BranchId)
                .ToListAsync();

            var users = await _context.BranchAccesses
                .Where(s => branch.Contains(s.BranchId))
                .ToListAsync();

            empIDs = users
                .GroupBy(s => new { s.EmployeeId })
                .Select(g => new BranchAccess() { EmployeeId = g.Key.EmployeeId })
                .Select(s => s.EmployeeId)
                .ToList();

            employeesAccess = await _context.Users
                .Where(s => empIDs.Contains(s.EmployeeId))
                .Select(s => s.LoginName)
                .ToListAsync();

            return employeesAccess;
        }
        private async Task<List<AdhocsAllDeleted>> AllDeletedAdhocs(List<ExceptionItemRev> list, DateTime? startDate, DateTime? cutOffDate, AdhocStatus status)
        {
            int adhocStatus = (int)status;
            List<AdhocsAllDeleted> delList = new List<AdhocsAllDeleted>();

            foreach (var item in list)
            {
                var subExceptions = item.ExceptionCodeRevs;
                var actionPlan = item.ActionPlans;

                if (actionPlan == null)
                    actionPlan = new List<ActionPlan>();
                foreach (var subEx in subExceptions)
                {
                    subEx.DeviationStatus = adhocStatus;
                    //if (subEx.DeviationStatus != (DeviationStatus)adhocStatus)
                    //{
                    //    continue;
                    //}

                    Deviation deviationLookup = await this.GetDeviations(subEx.ExCode);
                    string[] account = GetRevsAvailableAccount(item);
                    var agingResult = await ComputeAging(item.TransactionDate, item.ApprovedDateTime, subEx.SubReferenceNo, (DeviationStatus)subEx.DeviationStatus);

                    AdhocsAllDeleted del = new AdhocsAllDeleted();
                    del.ExceptionNo = subEx.SubReferenceNo;
                    del.BranchCode = item.BranchCode;
                    del.BranchName = item.BranchName;
                    del.Area = item.Area;
                    del.Division = item.Division;
                    del.TransactionDate = item.TransactionDate.ToString("MM/dd/yyyy");
                    del.DeletionDate = Convert.ToDateTime(item.ApprovedDateTime).ToString("MM/dd/yyyy");
                    del.Aging = agingResult.ToString();
                    del.AgingCategory = GetAgingCategory(Convert.ToDouble(del.Aging)).ToString();
                    del.Process = GetProcess(item);
                    del.AccountNo = account[0];
                    del.AccountName = account[1];
                    del.Deviation = deviationLookup.Deviation1;
                    del.RiskClassification = deviationLookup.RiskClassification;
                    del.DeviationCategory = deviationLookup.Category;
                    del.Amount = this.GetRevsAvailableAmount(item);
                    del.PersonResponsible = item.PersonResponsible;
                    del.OtherPersonResponsible = item.OtherPersonResponsible;
                    del.Remarks = item.Remarks;
                    del.DeletionRemarks = item.OtherRemarks;
                    del.ActionPlan = actionPlan == null || actionPlan.Count == 0 ? "" : actionPlan.FirstOrDefault().ActionPlan1;
                    del.EncodedBy = item.CreatedBy;
                    del.RootCause = ((RootCause)item.RootCause).GetDisplayName();
                    del.DeviationApprover = item.DeviationApprover;

                    delList.Add(del);
                }
            }

            #region Implement on Phase 2
            ////Add the exception code with "Delete From MERN" Changes
            //var deletedFromMERN = _context.ExceptionCodeRevs.Where(s => s.Changes == "Delete From MERN" && s.IsProcessed && s.ActionTaken == "Approved"
            //&& (s.DateCreated.Date >= startDate && s.DateCreated.Date <= cutOffDate));

            //foreach (var subEx in deletedFromMERN)
            //{
            //    var item = _context.ExceptionItemRev
            //                    .Include(s => s.ExCodeRevs)
            //                    .Include(s => s.ActionPlan)
            //                    .Include(s => s.MonetaryRevs)
            //                    .Include(s => s.NonMonetaryRevs)
            //                    .Include(s => s.MiscRevs)
            //                    .Where(s => s.RefNo == subEx.ExItemRefNo && s.IsProcessed && s.ActionTaken == "Approved").LastOrDefault();

            //    subEx.DeviationStatus = (DeviationStatus)adhocStatus;

            //    Deviations deviationLookup = GetDeviations(subEx.ExCode);
            //    string[] account = GetRevsAvailableAccount(item);

            //    AdhocsAllDeleted del = new AdhocsAllDeleted();
            //    del.ExceptionNo = subEx.SubReferenceNo;
            //    del.BranchCode = item.BranchCode;
            //    del.BranchName = item.BranchName;
            //    del.Area = item.Area;
            //    del.Division = item.Division;
            //    del.TransactionDate = subEx.DateCreated.ToString("MM/dd/yyyy");
            //    del.DeletionDate = Convert.ToDateTime(item.ApprovedDateTime).ToString("MM/dd/yyyy");
            //    del.Aging = ComputeAging(item.TransactionDate, item.ApprovedDateTime, subEx.SubReferenceNo, subEx.DeviationStatus).ToString();
            //    del.AgingCategory = item.AgingCategory.GetDisplayName();
            //    del.Process = GetProcess(item);
            //    del.AccountNo = account[0];
            //    del.AccountName = account[1];
            //    del.Deviation = deviationLookup.Deviation;
            //    del.RiskClassification = deviationLookup.RiskClassification;
            //    del.DeviationCategory = deviationLookup.Category;
            //    del.Amount = GetRevsAvailableAmount(item);
            //    del.PersonResponsible = item.PersonResponsible;
            //    del.OtherPersonResponsible = item.OtherPersonResponsible;
            //    del.Remarks = item.Remarks;
            //    del.DeletionRemarks = item.OtherRemarks;
            //    //del.ActionPlan = actionPlan == null || actionPlan.Count == 0 ? "" : actionPlan.FirstOrDefault().ActionPlan;
            //    del.EncodedBy = item.CreatedBy;
            //    del.RootCause = item.RootCause.GetDisplayName();
            //    del.DeviationApprover = item.DeviationApprover;

            //    delList.Add(del);
            //}
            #endregion


            return delList;
        }
        public async Task<Deviation> GetDeviations(int id)
        {
            return await _context.Deviations.SingleOrDefaultAsync(s => s.Id == id);
        }
        public AgingCategory GetAgingCategory(double age)
        {
            AgingCategory result = 0;

            if (age <= 7)
            {
                result = AgingCategory.LessEqual7Days;
            }
            else if (age <= 15)
            {
                result = AgingCategory.LessEqual15Days;
            }
            else if (age <= 30)
            {
                result = AgingCategory.LessEqual30Days;
            }
            else if (age <= 45)
            {
                result = AgingCategory.LessEqual45Days;
            }
            else if (age <= 180)
            {
                result = AgingCategory.LessEqual180Days;
            }
            else if (age <= 251)
            {
                result = AgingCategory.LessEqual1Year;
            }
            else if (age <= 502)
            {
                result = AgingCategory.LessEqual2Year;
            }
            else if (age <= 753)
            {
                result = AgingCategory.LessEqual3Year;
            }
            else if (age <= 1004)
            {
                result = AgingCategory.LessEqual4Year;
            }
            else if (age <= 1225)
            {
                result = AgingCategory.LessEqual5Year;
            }

            return result;
        }
        public async Task<double> ComputeAging(DateTime tranDate, DateTime? cutOffDate, string id, DeviationStatus status)
        {
            double age = 0;
            DateTime logicDate = (DateTime)cutOffDate;

            if (status != DeviationStatus.Outstanding)
            {
                // Get the date of approval of updated subexceptions minus transaction date
                var exCodeRevs = await _context.ExceptionCodeRevs
                                                .Where(s => s.SubReferenceNo == id && s.ActionTaken == "Approved")
                                                .OrderByDescending(s => s.DateCreated) // Use ordering to get the last one if needed
                                                .FirstOrDefaultAsync();

                if (cutOffDate == null)
                {
                    logicDate = (DateTime)cutOffDate;
                }

                if (exCodeRevs != null)
                {
                    if (exCodeRevs.TaggingDate != DateTime.MinValue)
                    {
                        logicDate = (DateTime)exCodeRevs.TaggingDate;
                    }
                    else
                    {
                        logicDate = (DateTime)exCodeRevs.ApprovedDateTime;
                    }
                }
            }

            age = (logicDate - tranDate).TotalDays;
            age = Math.Round(age);

            // Get Non Business Days
            DataTable dt = await this.GetBankNonBusinessDays(tranDate, logicDate); // Make this async if possible
            int nbdCount = dt.Rows.Count;
            age = (age - nbdCount);

            return age;
        }
        private string[] GetRevsAvailableAccount(ExceptionItemRev exItem)
        {
            string accNo = string.Empty;
            string accName = string.Empty;

            if (exItem.MonetaryRevs != null)
            {
                if (exItem.MonetaryRevs.CreditAccountNo != null && exItem.MonetaryRevs.CreditAccountNo != string.Empty)
                {
                    accNo = exItem.MonetaryRevs.CreditAccountNo;
                    accName = exItem.MonetaryRevs.CreditAccountName;
                }
                else
                {
                    accNo = exItem.MonetaryRevs.DebitAccountNo;
                    accName = exItem.MonetaryRevs.DebitAccountName;
                }

            }
            else if (exItem.NonMonetaryRevs != null)
            {
                if (exItem.NonMonetaryRevs.CustomerAccountNo != null)
                {
                    accNo = exItem.NonMonetaryRevs.CustomerAccountNo;
                }
                else
                {
                    accNo = exItem.NonMonetaryRevs.Cifnumber;
                }

                accName = exItem.NonMonetaryRevs.CustomerName;

            }
            else if (exItem.MiscRevs != null)
            {
                if (exItem.MiscRevs.BankCertNo != null)
                    accNo = exItem.MiscRevs.BankCertNo;
                else if (exItem.MiscRevs.CardNo != null)
                    accNo = exItem.MiscRevs.CardNo;
                else if (exItem.MiscRevs.CheckNo != null)
                    accNo = exItem.MiscRevs.CheckNo;
                else if (exItem.MiscRevs.Dpafno != null)
                    accNo = exItem.MiscRevs.Dpafno;
                else if (exItem.MiscRevs.GlslaccountNo != null)
                {
                    accNo = exItem.MiscRevs.GlslaccountNo;
                    accName = exItem.MiscRevs.GlslaccountName;
                }
            }

            string[] account = { accNo, accName };

            return account;
        }
        private string GetProcess(ExceptionItemRev revs)
        {
            string process = string.Empty;

            if (revs.MonetaryRevs != null)
            {
                process = revs.MonetaryRevs.TransDescription;
            }
            if (revs.NonMonetaryRevs != null)
            {
                NonMonetaryTypes type = (NonMonetaryTypes)revs.NonMonetaryRevs.Type;
                process = type.GetDisplayName();

            }
            if (revs.MiscRevs != null)
            {
                MiscTypes type = (MiscTypes)revs.MiscRevs.Type;
                process = type.GetDisplayName();
            }

            return process;
        }
        private decimal GetRevsAvailableAmount(ExceptionItemRev exItem)
        {
            decimal amt = 0;

            if (exItem.MonetaryRevs != null)
                amt = exItem.MonetaryRevs.Amount;
            else if (exItem.MiscRevs != null)
                amt = exItem.MiscRevs.Amount;

            return amt;
        }
        public string[] GetAvailableAccount(ExceptionItem exItem)
        {
            string accNo = string.Empty;
            string accName = string.Empty;
            Monetary MonetaryData = exItem.Monetaries.FirstOrDefault();
            NonMonetary NonMonetaryData = exItem.NonMonetaries.FirstOrDefault();
            Misc MiscData = exItem.Miscs.FirstOrDefault();
            if (MonetaryData != null)
            {
                if (MonetaryData.CreditAccountNo != null && MonetaryData.CreditAccountNo != string.Empty)
                {
                    accNo = MonetaryData.CreditAccountNo;
                    accName = MonetaryData.CreditAccountName;
                }
                else
                {
                    accNo = MonetaryData.DebitAccountNo;
                    accName = MonetaryData.DebitAccountName;
                }

            }
            else if (NonMonetaryData != null)
            {
                if (NonMonetaryData.CustomerAccountNo != null)
                    accNo = NonMonetaryData.CustomerAccountNo;
                else
                {
                    accNo = NonMonetaryData.Cifnumber;
                }


                accName = NonMonetaryData.CustomerName;

            }
            else if (MiscData != null)
            {
                if (MiscData.BankCertNo != null)
                    accNo = MiscData.BankCertNo;
                else if (MiscData.CardNo != null)
                    accNo = MiscData.CardNo;
                //else if (MiscData.CheckNo != null)
                //    accNo = MiscData.CheckNo;
                else if (MiscData.Dpafno != null)
                {
                    accNo = MiscData.Dpafno;
                    //accName = MiscData.GLSLAccountName; 
                }
                else if (MiscData.GlslaccountNo != null)
                {
                    accNo = MiscData.GlslaccountNo;
                    //accName = MiscData.GLSLAccountName;
                }

                if (!string.IsNullOrEmpty(MiscData.GlslaccountName))
                {
                    accName = MiscData.GlslaccountName;
                }
            }

            string[] account = { accNo, accName };

            return account;
        }
        public decimal GetAvailableAmount(ExceptionItem exItem)
        {
            Monetary MonetaryData = exItem.Monetaries.FirstOrDefault();
            Misc MiscData = exItem.Miscs.FirstOrDefault();
            decimal amt = 0;

            if (MonetaryData != null)
                amt = MonetaryData.Amount;
            else if (MiscData != null)
                amt = MiscData.Amount;

            return amt;
        }
        public string GetProcessForNonRevs(ExceptionItem revs)
        {
            string process = string.Empty;
            Monetary MonetaryData = revs.Monetaries.FirstOrDefault();
            NonMonetary NonMonetaryData = revs.NonMonetaries.FirstOrDefault();
            Misc MiscData = revs.Miscs.FirstOrDefault();

            if (MonetaryData != null)
            {
                process = MonetaryData.TransDescription;
            }
            if (NonMonetaryData != null)
            {
                NonMonetaryTypes type = (NonMonetaryTypes)NonMonetaryData.Category;
                process = type.GetDisplayName();

            }
            if (MiscData != null)
            {
                MiscTypes type = (MiscTypes)MiscData.Category;
                process = type.GetDisplayName();
            }

            return process;
        }
        private async Task<string> GetModifiedDate(string subErn, DeviationStatus status)
        {
            string result = string.Empty;
            var exCodeRevs = await _context.ExceptionCodeRevs
                .Where(s => s.SubReferenceNo == subErn && (DeviationStatus)s.DeviationStatus == status && s.Changes == "Update" && s.ActionTaken == "Approved")
                .OrderByDescending(s => s.ApprovedDateTime)
                .FirstOrDefaultAsync();

            if (exCodeRevs == null)
            {
                // Query for migrated data if initial exCodeRevs is null
                exCodeRevs = await _context.ExceptionCodeRevs
                    .Where(s => s.SubReferenceNo == subErn && (DeviationStatus)s.DeviationStatus == status && s.Changes == "Migrated")
                    .FirstOrDefaultAsync();
            }

            if (exCodeRevs != null)
            {
                if (exCodeRevs.TaggingDate != DateTime.MinValue)
                {
                    result = Convert.ToDateTime(exCodeRevs.TaggingDate).ToString("MM/dd/yyyy");
                }
                else
                {
                    result = Convert.ToDateTime(exCodeRevs.ApprovedDateTime).ToString("MM/dd/yyyy");
                }
            }

            return result;
        }
        private async Task<List<AdhocsOthers>> AdhocsOthers(List<ExceptionItem> list, AdhocStatus status, DateTime from, DateTime to)
        {
            int adhocStatus = (int)status;
            List<AdhocsOthers> adhocList = new List<AdhocsOthers>();
            try
            {

                foreach (var item in list)
                {
                    var subExceptions = item.ExceptionCodes;
                    var actionPlan = item.ActionPlans;
                    if (actionPlan == null)
                        actionPlan = new List<ActionPlan>();
                    foreach (var subEx in subExceptions)
                    {

                        if (subEx.DeviationStatus != adhocStatus)
                        {
                            continue;
                        }

                        Deviation deviationLookup = await this.GetDeviations(subEx.ExCode);
                        string[] account = this.GetAvailableAccount(item);
                        var agingResult = await ComputeAging(item.TransactionDate, to, subEx.SubReferenceNo, (DeviationStatus)subEx.DeviationStatus);

                        AdhocsOthers os = new AdhocsOthers();
                        os.ExceptionNo = subEx.SubReferenceNo;
                        os.BranchCode = item.BranchCode;
                        os.BranchName = item.BranchName;
                        os.Area = item.Area;
                        os.Division = item.Division;
                        os.TransactionDate = item.TransactionDate.ToString("MM/dd/yyyy");
                        os.DateModified = await GetModifiedDate(subEx.SubReferenceNo, (DeviationStatus)adhocStatus);
                        os.Aging = agingResult.ToString();
                        os.AgingCategory = GetAgingCategory(Convert.ToDouble(os.Aging)).ToString();
                        os.Process = GetProcessForNonRevs(item);
                        os.AccountNo = account[0];
                        os.AccountName = account[1];
                        os.Deviation = deviationLookup.Deviation1;
                        os.RiskClassification = deviationLookup.RiskClassification;
                        os.DeviationCategory = deviationLookup.Category;
                        os.Amount = GetAvailableAmount(item);
                        os.PersonResponsible = item.PersonResponsible;
                        os.OtherPersonResponsible = item.OtherPersonResponsible;
                        os.Remarks = item.Remarks;
                        os.ActionPlan = actionPlan == null || actionPlan.Count == 0 ? "" : actionPlan.FirstOrDefault().ActionPlan1;
                        os.EncodedBy = item.CreatedBy;
                        os.RootCause = ((RootCause)item.RootCause).GetDisplayName();
                        os.DeviationApprover = item.DeviationApprover;

                        adhocList.Add(os);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return adhocList;
        }
        private async Task<DataTable> GetBankNonBusinessDays(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var holidays = await _sibsDA.holiday_mast_tables
                      .FromSqlRaw("Select TO_CHAR(lchg_time,'MM-DD-YYYY') as lchg_time, " +
                                  " TO_CHAR(lchg_time,'Day') as lchg_Day, " +
                                  " Substr(mmyyyy,1,2) as mmyyyy, " +
                                  " hldy_str, ts_cnt" +
                                  " From tbaadm.holiday_mast_table" +
                                  " WHERE lchg_time BETWEEN TO_DATE('" + fromDate.ToString("yyyy-MM-dd") + "', 'YYYY-MM-DD') " +
                                  " AND TO_DATE('" + toDate.ToString("yyyy-MM-dd") + "', 'YYYY-MM-DD') " +
                                  " AND hldy_str = 'Y' " +
                                  " AND ts_cnt = '0'")
                      .ToListAsync();


                //var list = await _sibsDA.holiday_mast_tables
                //            //.Where(h => h.LchgTime.HasValue &&
                //            //            h.LchgTime.Value.Date >= fromDate.Date &&
                //            //            h.LchgTime.Value.Date <= toDate.Date &&
                //            //            h.HldyStr == "Y" &&
                //            //            h.TsCnt == 0)
                //            //.Select(h => new GetBankBusinessDays
                //            //{
                //            //    CDate = h.LchgTime.Value.ToString("MM-dd-yyyy"),
                //            //    CWDay = h.LchgTime.Value.ToString("dddd"),
                //            //    CMonth = h.MMYYYY.Substring(0, 2),
                //            //    BusDay = h.HldyStr,
                //            //    CBrNbr = h.TsCnt ?? 0
                //            //})
                //            .ToListAsync();
                return holidays.ToDataTable();
            }
            catch
            {
                throw;
            }
        }
        #endregion

    }
}
