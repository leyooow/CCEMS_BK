using Application.Contracts.Repositories;
using Application.Models.Helpers;
using Application.Models.Responses;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReportBranchReplyRepository : IReportBranchReplyRepository
    {
        private readonly CcemQatContext _context;
        public ReportBranchReplyRepository(CcemQatContext context)
        {
            _context = context;
        }
        public async Task<PaginatedList<ReportContent>> GetReportContentList(string id, string refNo, int page = 1)
        {
            var rContents = _context.ReportContents.Where(s => s.ExceptionNo == refNo);
            return await PaginatedList<ReportContent>.CreateAsync(rContents.AsNoTracking(), page);
        }
        public async Task<List<BranchReply>> GetBranchReplyList(string id)
        {
            return await _context.BranchReplies.Where(s => s.ReportContentsId == Guid.Parse(id)).OrderByDescending(s => s.DateCreated).ToListAsync();
        }
        public async Task<ReportContent> GetReportContents(Guid ReportContentsId)
        {
            return await _context.ReportContents.SingleOrDefaultAsync(s => s.Id == ReportContentsId);
        }
        public async Task SavePostReply(ReportContent rContent, BranchReply bReply)
        {
            _context.Add(bReply);
            _context.Update(rContent);
            await _context.SaveChangesAsync();
        }
    }
}
