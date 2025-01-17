using Application.Models.DTOs.Report;
using Application.Models.Helpers;
using Application.Models.Responses;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Services
{
    public interface IReportDetailsService
    {
        Task<PaginatedList<ReportContent>> GetReportContentsList(int id, int page);
        Task<ReportDTO> GetReport(int id);
        Task<List<string>> SelectedBranches(int id);
        Task<string[]> GetBranchNames(Report report);
        Task<List<DropdownReturn>> PopulateBranchRecipients(string selected = "", string brCode = "");
        Task<GenericResponse<EPPlusReturn>> PulloutRequest(int id, string refno);
        Task<GenericResponse<EPPlusReturn>> ExportDataFromDetails(int id);
        Task<GenericResponse<dynamic>> Reject(int id, Guid reportsGuid, string remarks);
        Task<GenericResponse<dynamic>> Approve(int id, Guid reportsGuid);
        Task<GenericResponse<dynamic>> SendReport(List<string> ToList, List<string> CCList, int id);
    }
}
