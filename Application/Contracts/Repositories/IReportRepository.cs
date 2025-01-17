using Application.Models.DTOs.Report;
using Application.Models.Helpers;
using Application.Models.Responses;
using Infrastructure.Entities;

namespace Application.Contracts.Repositories
{
    public interface IReportRepository
    {
        IQueryable<Report> GetList(string searchString, List<string> employeesAccess);
        IQueryable<Report> GetListFilterWithPendingAndApproved(string searchString, List<string> employeesAccess);
        IQueryable<Report> GetListFilterWithApproved(string searchString, List<string> employeesAccess);
        IQueryable<Report> GetEmptyList();
        Task<List<string>> GetEmeployeesWithAccess(string empID);
        Task<List<Group>>PopulateGroupsDropDownList(string empID);
        Task<string> GetDeviationDesc(int Id);
        Task<string> GetDeviationCategoryDesc(int Id);
        Task<double> CalculateOutstandingDays(DateTime tranDate, DateTime? dateModified, DateTime cutOffDate);
        Task<GenericResponse<EPPlusReturn>> GeneratePervasivenessLogic(DownloadAdhocViewModel vm);
        Task<GenericResponse<EPPlusReturn>> GenerateRegularizationTATLogic(DownloadAdhocViewModel vm);
        Task<GenericResponse<EPPlusReturn>> GenerateExceptionAdhocsLogic(DownloadAdhocViewModel vm, string empID);
        string[] GetAvailableAccount(ExceptionItem exItem);
        Task<Deviation> GetDeviations(int id);
        Task<double> ComputeAging(DateTime tranDate, DateTime? cutOffDate, string id, DeviationStatus status);
        string GetProcessForNonRevs(ExceptionItem revs);
        AgingCategory GetAgingCategory(double age);
        decimal GetAvailableAmount(ExceptionItem exItem);
    }
}
