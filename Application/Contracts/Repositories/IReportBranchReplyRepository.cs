using Application.Models.Helpers;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IReportBranchReplyRepository
    {
        Task<PaginatedList<ReportContent>> GetReportContentList(string id, string refNo, int page = 1);
        Task<List<BranchReply>> GetBranchReplyList(string id);
        Task<ReportContent> GetReportContents(Guid ReportContentsId);
        Task SavePostReply(ReportContent rContent, BranchReply bReply);
    }
}
