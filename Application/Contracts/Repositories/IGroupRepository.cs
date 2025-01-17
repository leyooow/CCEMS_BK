using Application.Models.DTOs;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IGroupRepository : IBaseRepository<Group>
    {
        Task<List<Group>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm);
        Task<bool> CheckExistingGroupNameAsync(string? name);
        Task<Group?> GetGroupByCode(string? code);
        Task<List<string>> GetCurrentUserBranchIds();
    }
}
