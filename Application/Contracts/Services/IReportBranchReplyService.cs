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
    public interface IReportBranchReplyService
    {
        Task<PaginatedList<ReportContent>> GetReportContentList(string id, string refNo, int page = 1);
        Task<List<BranchReply>> GetBranchReplyList(string id);
        Task<GenericResponse<dynamic>> PostReply(BranchReplyViewModel vm);
    }
}
