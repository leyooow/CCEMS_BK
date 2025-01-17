using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Report;
using Application.Models.Helpers;
using Application.Models.Responses;
using Application.Services.Application.Services;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReportGenerateService : IReportGenerateService
    {
        private readonly IReportGenerateRepository _repository;
        private readonly UserClaimsDTO _user;
        private readonly IUserClaimsService _userClaimsService;
        private readonly string basePath = "C:\\CCEM\\Reports\\"; //parameterize
        public ReportGenerateService(IReportGenerateRepository repository, IUserClaimsService userClaimsService) 
        {
            _repository = repository;
            _userClaimsService = userClaimsService;
            _user = _userClaimsService.GetClaims();
        }
        public Task<List<DropdownReturn>> PopulateGroupsDropDownList()
        {
            string EmployeeID = _user.EmployeeID ?? string.Empty;
            return _repository.PopulateGroupsDropDownList(EmployeeID);
        }
        public async Task<GenericResponse<dynamic>> GenerateReport(GenerateMainReportsViewModel GenerateReports)
        {
            string loggedRole = _user.RoleName ?? "";
            string Username = _user.LoginName; 
            var coverage = GenerateReports.ReportCoverage;
            List<string> SelectedBranches = GenerateReports.SelectedBranches;
            try
            {
                if (SelectedBranches != null)
                {
                    int category = 0;
                    switch ((ReportCoverage)coverage)
                    {
                        case ReportCoverage.Daily:

                            category = (int)GenerateReports.DailyCategory;
                            GenerateReports.ReportCategory = (ReportCategory)category;
                            if (category == (int)DailyCategory.DailyExceptionReport)
                            {
                                return await ProcessDailyOutstanding(GenerateReports, SelectedBranches, loggedRole, Username);
                            }
                            else if (category == (int)DailyCategory.RedFlag)
                            {
                                return await ProcessDailyRedFlag(GenerateReports, SelectedBranches, loggedRole, Username);
                            }
                            break;

                        case ReportCoverage.Weekly:

                            category = (int)GenerateReports.WeeklyCategory;
                            GenerateReports.ReportCategory = (ReportCategory)category;

                            if (category == (int)WeeklyCategory.Escalation)
                            {
                                return await ProcessWeeklyEscalation(GenerateReports, SelectedBranches, loggedRole, Username);
                            }
                            if (category == (int)WeeklyCategory.NewAccounts)
                            {
                                return await ProcessWeeklyNewAccounts(GenerateReports, SelectedBranches, loggedRole, Username);
                            }
                            break;

                        case ReportCoverage.Monthly:

                            category = (int)GenerateReports.MonthlyCategory;
                            GenerateReports.ReportCategory = (ReportCategory)category;

                            if (category == (int)MonthlyCategory.AllOutstanding1)
                            {
                                return await ProcessAllOutstanding(GenerateReports, SelectedBranches, loggedRole, Username);
                            }
                            if (category == (int)MonthlyCategory.AllOutstanding2)
                            {
                                return await ProcessAllOutstanding(GenerateReports, SelectedBranches, loggedRole, Username);
                            }
                            break;

                        default:
                            break;
                    }
                    return ResponseHelper.ErrorResponse<dynamic>("Error");
                }
                else
                {
                    return ResponseHelper.ErrorResponse<dynamic>("Please select atleast 1 Branch");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<GenericResponse<dynamic>> ProcessDailyOutstanding(GenerateMainReportsViewModel GenerateReports, List<string> SelectedBranches, string loggedRole, string Username)
        {
            string errMsg = string.Empty;
            DateTime dateFrom = GenerateReports.DateFrom;
            DateTime dateTo = GenerateReports.DateTo;
            DateTime dateCoverage = GenerateReports.DateCoverage;

            try
            {
                // Monetary and Miscellaneous only
                List<ExceptionItem> exception = await _repository.GetProcessDailyOutstanding(dateCoverage);

                if (exception != null && exception.Count != 0)
                {
                    var branchCodes = SelectedBranches;

                    List<string> filteredBrCodes = new List<string>();

                    foreach (var code in branchCodes)
                    {
                        //Avoid duplicates
                        if (!filteredBrCodes.Contains(code))
                        {
                            filteredBrCodes.Add(code.PadLeft(3, '0'));
                        }
                    }
                    string concatinatedBrcodes = ConcatinateBranchCodes(filteredBrCodes);

                    var filteredException = exception.Where(s => branchCodes.Contains(s.BranchCode)).ToList();

                    // Process Excel File.
                    string[] fileDetails = { "", "", dateCoverage.ToString("yyyy-MM-dd")
                            , concatinatedBrcodes, "Approved", "Create", "true" };

                    // If Error, prevent database insert.
                    if (errMsg == string.Empty)
                    {
                        // Save Details to DB.
                        List<ReportContent> rContents = await _repository.OutstandingDaily(filteredException, dateTo);

                        errMsg = await _repository.InsertReportRecordsToDB(fileDetails, (ReportCategory)GenerateReports.ReportCategory, (ReportCoverage)GenerateReports.ReportCoverage, ReportStatus.Standby, filteredException, null, rContents, loggedRole, Username);

                        if (errMsg == string.Empty)
                        {
                            return ResponseHelper.SuccessResponse<dynamic>("Successfully Generated!");
                        }
                        else
                        {
                            return ResponseHelper.ErrorResponse<dynamic>(errMsg);
                        }
                    }
                    else
                    {
                        return ResponseHelper.ErrorResponse<dynamic>(errMsg);
                    }
                }
                else
                {
                    return ResponseHelper.ErrorResponse<dynamic>("No exceptions from the selected date coverage");
                }

            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
            }
        }
        private async Task<GenericResponse<dynamic>> ProcessDailyRedFlag(GenerateMainReportsViewModel GenerateReports, List<string> SelectedBranches, string loggedRole, string Username)
        {
            string errMsg = string.Empty;
            DateTime dateFrom = GenerateReports.DateFrom;
            DateTime dateTo = GenerateReports.DateTo;
            DateTime dateCoverage = GenerateReports.DateCoverage;

            try
            {
                var exception = await _repository.GetProcessDailyRedFlag(dateCoverage);

                if (exception != null && exception.Count() != 0)
                {
                    var branchCodes = SelectedBranches;

                    List<string> filteredBrCodes = new List<string>();
                    foreach (var code in branchCodes)
                    {
                        //Avoid duplicates
                        if (!filteredBrCodes.Contains(code))
                        {
                            filteredBrCodes.Add(code.PadLeft(3, '0'));
                        }
                    }
                    string concatinatedBrcodes = ConcatinateBranchCodes(filteredBrCodes);

                    // Create Folder - ~BasePath/Daily/BranchCode/Date/File                   
                    //string directory = string.Format("{0}{1}\\{2}\\{3}\\", basePath, "Daily", concatinatedBrcodes, dateCoverage.ToString("yyyyMMdd"));
                    //string fileName = string.Format("Daily_RedFlag[{0}]_{1}.xlsx", concatinatedBrcodes, dateCoverage.ToString("yyyyMMdd"));

                    var filteredException = exception.Where(s => branchCodes.Contains(s.BranchCode)).ToList();

                    List<ReportContent> formattedException = await _repository.RedFlagDaily(filteredException, dateTo);

                    // Process Excel File.
                    string[] fileDetails = { "", "", dateCoverage.ToString("yyyy-MM-dd")
                            , concatinatedBrcodes, "Approved", "Create", "true" };

                    //errMsg = _package.DailyRedFlagPackage(formattedException, fileDetails);

                    // If Error, prevent database insert.
                    if (errMsg == string.Empty)
                    {
                        // Save Details to DB.
                        errMsg = await _repository.InsertReportRecordsToDB(fileDetails, (ReportCategory)GenerateReports.ReportCategory, (ReportCoverage)GenerateReports.ReportCoverage, ReportStatus.Standby, filteredException, null, formattedException, loggedRole, Username);

                        if (errMsg == string.Empty)
                        {
                            return ResponseHelper.SuccessResponse<dynamic>("Successfully Generated!");
                        }
                        else
                        {
                            return ResponseHelper.ErrorResponse<dynamic>(errMsg);
                        }
                    }
                    else
                    {
                        return ResponseHelper.ErrorResponse<dynamic>(errMsg);
                    }
                }
                else
                {
                    return ResponseHelper.ErrorResponse<dynamic>("No exceptions from the selected date coverage");
                }

            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
            }
        }
        private async Task<GenericResponse<dynamic>> ProcessWeeklyEscalation(GenerateMainReportsViewModel GenerateReports, List<string> SelectedBranches, string loggedRole, string Username)
        {
            string errMsg = string.Empty;
            DateTime dateFrom = GenerateReports.DateFrom;
            DateTime dateTo = GenerateReports.DateTo;
            DateTime dateCoverage = GenerateReports.DateCoverage;

            try
            {
                var exception = await _repository.GetProcessWeeklyEscalation(dateFrom,dateTo);

                if (exception != null && exception.Count() != 0)
                {
                    var branchCodes = SelectedBranches;

                    List<string> filteredBrCodes = new List<string>();
                    foreach (var code in branchCodes)
                    {
                        //Avoid duplicates
                        if (!filteredBrCodes.Contains(code))
                        {
                            filteredBrCodes.Add(code.PadLeft(3, '0'));
                        }
                    }
                    string concatinatedBrcodes = ConcatinateBranchCodes(filteredBrCodes);

                    // Create Folder - ~BasePath/Daily/BranchCode/Date/File                   
                    //string directory = string.Format("{0}{1}\\{2}\\{3}\\", basePath, "Weekly", concatinatedBrcodes, dateCoverage.ToString("yyyyMMdd"));
                    //string fileName = string.Format("Escalation_Report[{0}]_{1}.xlsx", concatinatedBrcodes, dateCoverage.ToString("yyyyMMdd"));

                    var filteredException = exception.Where(s => branchCodes.Contains(s.BranchCode)).ToList();

                    List<ReportContent> formattedException = await _repository.EscalationWeekly(filteredException, dateTo);

                    // Process Excel File.
                    string[] fileDetails = { "", "", dateCoverage.ToString("yyyy-MM-dd")
                            , concatinatedBrcodes, "Approved", "Create", "true" };

                    //errMsg = _package.WeeklyEscalationPackage(formattedException, fileDetails);

                    // If Error, prevent database insert.
                    if (errMsg == string.Empty)
                    {
                        // Save Details to DB.
                        errMsg = await _repository.InsertReportRecordsToDB(fileDetails, (ReportCategory)GenerateReports.ReportCategory, (ReportCoverage)GenerateReports.ReportCoverage, ReportStatus.Standby, filteredException, null, formattedException, loggedRole, Username);

                        if (errMsg == string.Empty)
                        {
                            return ResponseHelper.SuccessResponse<dynamic>("Successfully Generated!");
                        }
                        else
                        {
                            return ResponseHelper.ErrorResponse<dynamic>(errMsg);
                        }
                    }
                    else
                    {
                        return ResponseHelper.ErrorResponse<dynamic>(errMsg);
                    }
                }
                else
                {
                    return ResponseHelper.ErrorResponse<dynamic>("No exceptions from the selected date coverage");
                }

            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
            }
        }
        private async Task<GenericResponse<dynamic>> ProcessAllOutstanding(GenerateMainReportsViewModel GenerateReports, List<string> SelectedBranches, string loggedRole, string Username)
        {
            string errMsg = string.Empty;
            DateTime dateFrom = GenerateReports.DateFrom;
            DateTime dateTo = GenerateReports.DateTo;
            DateTime dateCoverage = GenerateReports.DateCoverage;

            List<ExceptionItem> exception = new List<ExceptionItem>();

            try
            {
                if (GenerateReports.MonthlyCategory == MonthlyCategory.AllOutstanding1)
                {
                    // Monetary and Miscellaneous
                    exception = await _repository.GetProcessAllOutstandingMonetaryAndMiscellaneous(dateFrom, dateTo);
                }
                else
                {
                    exception = await _repository.GetProcessAllOutstandingNonMonetary(dateFrom, dateTo);
                }

                var tranType = GenerateReports.MonthlyCategory;

                if (exception != null && exception.Count != 0)
                {
                    var branchCodes = SelectedBranches;

                    List<string> filteredBrCodes = new List<string>();

                    foreach (var code in branchCodes)
                    {
                        //Avoid duplicates
                        if (!filteredBrCodes.Contains(code))
                        {
                            filteredBrCodes.Add(code.PadLeft(3, '0'));
                        }
                    }
                    string concatinatedBrcodes = ConcatinateBranchCodes(filteredBrCodes);

                    // Create Folder - ~BasePath/Daily/BranchCode/Date/File                   
                    string directory = string.Format("{0}{1}\\{2}\\{3}\\", basePath, "Monthly", concatinatedBrcodes, dateCoverage.ToString("yyyyMMdd"));

                    string fileName = tranType == MonthlyCategory.AllOutstanding1 ?
                        string.Format("All Outstanding Report for Monetary and Misc [{0}]_{1}.xlsx", concatinatedBrcodes, dateCoverage.ToString("yyyyMMdd")) :
                        string.Format("All Outstanding Report for Non Monetary [{0}]_{1}.xlsx", concatinatedBrcodes, dateCoverage.ToString("yyyyMMdd"));

                    var filteredException = exception.Where(s => branchCodes.Contains(s.BranchCode)).ToList();

                    List<ReportContent> formattedException = await _repository.AllOutstandingMonthly(filteredException, dateTo);

                    // Process Excel File.
                    string[] fileDetails = { directory, fileName, dateCoverage.ToString("yyyy-MM-dd")
                            , concatinatedBrcodes, "Approved", "Create", "true" };

                    //errMsg = _package.MonthlyOutstandingPackage(formattedException, fileDetails, tranType);

                    // If Error, prevent database insert.
                    if (errMsg == string.Empty)
                    {
                        // Save Details to DB.
                        errMsg = await _repository.InsertReportRecordsToDB(fileDetails, (ReportCategory)GenerateReports.ReportCategory, (ReportCoverage)GenerateReports.ReportCoverage, ReportStatus.Standby, filteredException, null, formattedException, loggedRole, Username);

                        if (errMsg == string.Empty)
                        {
                            return ResponseHelper.SuccessResponse<dynamic>("Successfully Generated!");
                        }
                        else
                        {
                            return ResponseHelper.ErrorResponse<dynamic>(errMsg);
                        }
                    }
                    else
                    {
                        return ResponseHelper.ErrorResponse<dynamic>(errMsg);
                    }
                }
                else
                {
                    return ResponseHelper.ErrorResponse<dynamic>("No exceptions from the selected date coverage");
                }

            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
            }
        }
        private async Task<GenericResponse<dynamic>> ProcessWeeklyNewAccounts(GenerateMainReportsViewModel GenerateReports, List<string> SelectedBranches, string loggedRole, string Username)
        {
            string errMsg = string.Empty;
            DateTime dateFrom = GenerateReports.DateFrom;
            DateTime dateTo = GenerateReports.DateTo;
            DateTime dateCoverage = GenerateReports.DateCoverage;

            try
            {
                var exception = await _repository.GetProcessWeeklyNewAccounts(dateFrom,dateTo);

                if (exception != null && exception.Count() != 0)
                {
                    var branchCodes = SelectedBranches;

                    List<string> filteredBrCodes = new List<string>();
                    foreach (var code in branchCodes)
                    {
                        //Avoid duplicates
                        if (!filteredBrCodes.Contains(code))
                        {
                            filteredBrCodes.Add(code.PadLeft(3, '0'));
                        }
                    }
                    string concatinatedBrcodes = ConcatinateBranchCodes(filteredBrCodes);

                    // Create Folder - ~BasePath/Daily/BranchCode/Date/File                   
                    //string directory = string.Format("{0}{1}\\{2}\\{3}\\", basePath, "Weekly", concatinatedBrcodes, dateCoverage.ToString("yyyyMMdd"));
                    //string fileName = string.Format("New Accounts Report[{0}]_{1}.xlsx", concatinatedBrcodes, dateCoverage.ToString("yyyyMMdd"));

                    var filteredException = exception.Where(s => branchCodes.Contains(s.BranchCode)).ToList();

                    List<ReportContent> formattedException = await _repository.NewAccountsWeekly(filteredException, dateTo);

                    // Process Excel File.
                    string[] fileDetails = { "", "", dateCoverage.ToString("yyyy-MM-dd")
                            , concatinatedBrcodes, "Approved", "Create", "true" };

                    //errMsg = _package.WeeklyNewAccountsPackage(formattedException, fileDetails);

                    // If Error, prevent database insert.
                    if (errMsg == string.Empty)
                    {
                        // Save Details to DB.
                        errMsg = await _repository.InsertReportRecordsToDB(fileDetails, (ReportCategory)GenerateReports.ReportCategory, (ReportCoverage)GenerateReports.ReportCoverage, ReportStatus.Standby, filteredException, null, formattedException, loggedRole, Username);

                        if (errMsg == string.Empty)
                        {
                            return ResponseHelper.SuccessResponse<dynamic>("Successfully Generated!");
                        }
                        else
                        {
                            return ResponseHelper.ErrorResponse<dynamic>(errMsg);
                        }
                    }
                    else
                    {
                        return ResponseHelper.ErrorResponse<dynamic>(errMsg);
                    }
                }
                else
                {
                    return ResponseHelper.ErrorResponse<dynamic>("No exceptions from the selected date coverage");
                }

            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
            }
        }
        public static string ConcatinateBranchCodes(List<string> brCodes)
        {
            string branchIDs = string.Empty;
            brCodes = brCodes.OrderBy(s => s).ToList();

            foreach (var item in brCodes)
            {
                if (branchIDs != string.Empty)
                    branchIDs += ("-" + item);
                else
                    branchIDs += item;
            }

            return branchIDs;
        }
    }
}
