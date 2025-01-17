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
    public interface IReportGenerateService
    {
        Task<List<DropdownReturn>> PopulateGroupsDropDownList();
        Task<GenericResponse<dynamic>> GenerateReport(GenerateMainReportsViewModel GenerateReports);
    }
}
