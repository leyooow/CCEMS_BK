using Application.Models.DTOs.Common;
using Application.Models.DTOs.Group;
using Application.Models.DTOs.User.role;
using Application.Models.DTOs.User.user;
using Application.Models.Responses;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Services
{
    public interface IUserService
    {
        Task<GenericResponse<List<UserDTO>>> GetAllAsync();
        Task<GenericResponse<PagedResult<UserDTO>>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm);
        Task<GenericResponse<UserDTO?>> GetUserByIdAsync(string? id);
        Task<GenericResponse<List<RoleDTO>>> GetAllRolesAsync();
        Task<GenericResponse<List<PermissionLookup>>> GetAllPermissionLookUpAsync();
        Task<GenericResponse<List<RolePermission>>> GetPermissionsByRoleId(int roleId);
        Task<GenericResponse<object>> AddPermissionsAsync(AddPermissionRequest addPermissionRequest );
        Task<GenericResponse<object>> AddUserAsync(UserCreateDTO userCreateDTO);
        Task<GenericResponse<UserActiveDirectoryDTO>> CheckUserNameAsync(string userName);
        Task<GenericResponse<object>> UpdateUserAsync(UserUpdateDTO userUpdateDTO);
        Task<GenericResponse<object>> DeleteUserAsync(string employeeId);
        //Task<GenericResponse<object>> AddGroupAsync(GroupCreateDTO groupCreateDto);
        //Task<GenericResponse<object>> UpdateGroupAsync(GroupUpdateDTO groupUpdateDto);
        //Task<GenericResponse<object>> DeleteGroupAsync(int id);
    }
}
