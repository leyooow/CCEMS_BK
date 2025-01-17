using Application.Models.DTOs.User.role;
using Application.Models.Responses;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<List<User>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm);
        Task<User?> GetUserByIdAsync(string? id);
        Task<List<Role>> GetAllRolesAsync();
        Task<List<PermissionLookup>> GetAllPermissionLookUpAsync();
        Task<List<RolePermission>> GetPermissionsByRoleId(int roleId);
        Task AddPermissionsAsync(AddPermissionRequest addPermissionRequest);
        Task<bool> IsUserExistingAsync(string username);
        Task UpdateUserAsync(User user, List<BranchAccess> updatedBranchAccesses, List<int> branchAccessIds);
        Task DeleteUserAsync(string employeeId);

        //Task<User> GetUserWithBranchAccessesAsync(string employeeId);
    }
}
