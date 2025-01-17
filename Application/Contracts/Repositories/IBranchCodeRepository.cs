using Domain.FEntities;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IBranchCodeRepository 
    {
        Task<List<BranchCodeTable>> GetAllAsync();
        Task<BranchCodeTable> GetByIdAsync(string? code);
        Task<List<BranchCodeTable>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm);
        Task<int> GetTotalCountAsync(string? searchTerm);
    }
}
