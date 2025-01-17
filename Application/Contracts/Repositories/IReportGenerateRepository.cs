using Application.Models.DTOs.Report;
using Application.Models.Helpers;
using Application.Models.Responses;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IReportGenerateRepository
    {
        Task<List<DropdownReturn>> PopulateGroupsDropDownList(string EmployeeID);
        Task<string> InsertReportRecordsToDB(string[] fileDetails, ReportCategory category, ReportCoverage coverage, ReportStatus status,
            List<ExceptionItem> exception, List<ExceptionItemRev> exceptionRevs, List<ReportContent> rContents, string loggedRole, string Username);
        Task<List<ReportContent>> RedFlagDaily(List<ExceptionItem> list, DateTime? cutOffDate);
        Task<List<ReportContent>> OutstandingDaily(List<ExceptionItem> list, DateTime? cutOffDate);
        Task<List<ReportContent>> NewAccountsWeekly(List<ExceptionItem> list, DateTime? cutOffDate);
        Task<List<ReportContent>> EscalationWeekly(List<ExceptionItem> list, DateTime? cutOffDate);
        Task<List<ReportContent>> AllOutstandingMonthly(List<ExceptionItem> list, DateTime? cutOffDate);
        Task<List<ExceptionItem>> GetProcessDailyOutstanding(DateTime dateCoverage);
        Task<List<ExceptionItem>> GetProcessDailyRedFlag(DateTime dateCoverage);
        Task<List<ExceptionItem>> GetProcessWeeklyEscalation(DateTime dateFrom, DateTime dateTo);
        Task<List<ExceptionItem>> GetProcessAllOutstandingMonetaryAndMiscellaneous(DateTime dateFrom, DateTime dateTo);
        Task<List<ExceptionItem>> GetProcessAllOutstandingNonMonetary(DateTime dateFrom, DateTime dateTo);
        Task<List<ExceptionItem>> GetProcessWeeklyNewAccounts(DateTime dateFrom, DateTime dateTo);
    }
}
