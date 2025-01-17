using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IAuditLogRepository
    {
        //Task SaveLogAsync(AuditLog log);
        //Task<List<AuditLog>> GetLogsAsync();
        //Task<List<AuditLog>> GetLogsByUserAsync(string username);
        Task<List<AuditLog>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm);
        Task<int> GetTotalCountAsync(string? searchTerm);
    }
}
