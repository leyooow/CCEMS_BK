using Application.Contracts.Repositories;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        private readonly CcemQatContext _context;

        public AuditLogRepository(CcemQatContext context) : base(context)
        {
            _context = context;
        }

        //public async Task SaveLogAsync(AuditLog log)
        //{
        //    //await base.AddAsync();
        //    _context.AuditLogs.Add(log);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task<List<AuditLog>> GetLogsAsync()
        //{
        //    return await _context.AuditLogs.ToListAsync();
        //}

        //public async Task<List<AuditLog>> GetLogsByUserAsync(string username)
        //{
        //    return await _context.AuditLogs
        //        .Where(log => log.ActionBy == username)
        //        .ToListAsync();
        //}

        public async Task<List<AuditLog>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm)
        {
            IQueryable<AuditLog> query = _context.Set<AuditLog>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(g =>
                    g.ModuleName.Contains(searchTerm) ||
                    g.ActionType.Contains(searchTerm) ||
                    g.ActionDesc.Contains(searchTerm) ||
                    g.ActionDesc.Contains(searchTerm) ||
                    g.DateEntry.ToString().Contains(searchTerm));
            }

            query = query.OrderByDescending(x => x.DateEntry);

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                query = query
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? searchTerm)
        {

            return await base.GetTotalCountAsync(searchTerm);
        }
    }

}
