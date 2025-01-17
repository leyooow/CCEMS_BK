using Application.Models.DTOs.Common;
using Application.Models.DTOs.Employee;
using Application.Models.DTOs.FBranch;
using Application.Models.Responses;
using Domain.FEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Services
{
    public interface IBranchCodeService
    {
        Task<GenericResponse<List<BranchCodeTableDTO>>> GetAllAsync();
        Task<BranchCodeTable> GetByIdAsync(string? code);
        Task<GenericResponse<PagedResult<BranchCodeTableDTO>>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm);
    }
}
