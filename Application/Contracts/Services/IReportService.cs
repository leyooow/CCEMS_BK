using Application.Models;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Group;
using Application.Models.DTOs.Report;
using Application.Models.DTOs.User;
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
    public interface IReportService
    {
        Task<PaginatedList<ReportDTO>> GetList(string searchString, int Page);
        Task<List<Group>> PopulateGroupsDropDownList();
        Task<GenericResponse<EPPlusReturn>> DownloadAdhoc(DownloadAdhocViewModel vm);
    }
}
