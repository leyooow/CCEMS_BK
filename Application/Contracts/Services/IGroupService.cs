using Application.Models.DTOs.Common;
using Application.Models.DTOs.Group;
using Application.Models.DTOs.User;
using Application.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Services
{
    public interface IGroupService
    {
        Task<GenericResponse<List<GroupDTO>>> GetAllAsync();
        Task<GenericResponse<PagedResult<GroupDTO>>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm);
        Task<GenericResponse<GroupDTO?>> GetGroupByIdAsync(int id);
        Task<List<GroupDTO>> GetBranchDetailsAsync(IEnumerable<string> branchIds);
        Task<GenericResponse<object>> AddGroupAsync(GroupCreateDTO groupCreateDto);
        Task<GenericResponse<object>> UpdateGroupAsync(GroupUpdateDTO groupUpdateDto);
        Task<GenericResponse<object>> DeleteGroupAsync(int id);
        Task<List<GroupDTO>> GetBranchDropdown();
    }
}
