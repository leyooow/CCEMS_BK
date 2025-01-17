using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Group;
using Application.Models.DTOs.User.role;
using Application.Models.DTOs.User.user;
using Application.Models.Helpers;
using Application.Models.Responses;
using AutoMapper;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }

        public async Task<GenericResponse<List<UserDTO>>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();

            var userDTOs = _mapper.Map<List<UserDTO>>(users);

            return ResponseHelper.SuccessResponse(userDTOs, "Users retrieved succesfully.");
        }

        public async Task<GenericResponse<UserDTO?>> GetUserByIdAsync(string id)
        {
            try
            {
                var User = await _repository.GetUserByIdAsync(id);
                var mapUser = User == null ? null : _mapper.Map<UserDTO>(User);

                return ResponseHelper.SuccessResponse(mapUser, "User retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<UserDTO?>($"An error occurred while retrieving the group: {ex.Message}");
            }
        }

        public async Task<GenericResponse<PagedResult<UserDTO>>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm)
        {
            try
            {
                var Users = await _repository.GetPaginatedAsync(pageNumber, pageSize, searchTerm);
                var UserDtos = _mapper.Map<List<UserDTO>>(Users);
                var userRoles = await _repository.GetAllRolesAsync();

                // Map role names to UserDTOs
                foreach (var userDto in UserDtos)
                {
                    var userRole = userRoles
                        .SingleOrDefault(r => r.Id == userDto.RoleId); // Match UserRoleId

                    userDto.RoleName = userRole?.RoleName ?? "Unknown"; // Handle null case
                }
                //var branchCount = UserDtos.Select(u => u.BranchAccesses).Count();

                // Get the total count of Users for pagination metadata
                var totalCount = await _repository.GetTotalCountAsync(searchTerm);

                var pagedResult = new PagedResult<UserDTO>
                {
                    Items = UserDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber ?? 1,  // Default to 1 if not provided
                    PageSize = pageSize ?? 10      // Default to 10 if not provided
                };

                return ResponseHelper.SuccessResponse(pagedResult, "Paginated Users retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<PagedResult<UserDTO>>($"Failed to retrieve groups: {ex.Message}");
            }
        }

        public async Task<GenericResponse<List<RoleDTO>>> GetAllRolesAsync()
        {
            var roles = await _repository.GetAllRolesAsync();

            var rolesDTOs = _mapper.Map<List<RoleDTO>>(roles);

            return ResponseHelper.SuccessResponse(rolesDTOs, "Roles retrieved successfully");
        }

        public async Task<GenericResponse<List<PermissionLookup>>> GetAllPermissionLookUpAsync()
        {
            var permissionLookups = await _repository.GetAllPermissionLookUpAsync();

            var permissionLookupDTOs = _mapper.Map<List<PermissionLookup>>(permissionLookups);

            return ResponseHelper.SuccessResponse(permissionLookupDTOs, "Permission Lookup retrieved successfully");
        }
        public async Task<GenericResponse<List<RolePermission>>> GetPermissionsByRoleId(int roleId)
        {
            var rolePermissions = await _repository.GetPermissionsByRoleId(roleId);
            var rolePermissionDTOs = _mapper.Map<List<RolePermission>>(rolePermissions);

            return ResponseHelper.SuccessResponse(rolePermissionDTOs, "Role Permissions retrieved successfully");
        }

        public async Task<GenericResponse<object>> AddPermissionsAsync(AddPermissionRequest addPermissionRequest)
        {
            try
            {
                await _repository.AddPermissionsAsync(addPermissionRequest);
                return ResponseHelper.SuccessResponse<object>(null, "Permissions added successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<object>($"Error in saving permissions, Error {ex.Message}");
            }
        }

        public async Task<GenericResponse<object>> AddUserAsync(UserCreateDTO userCreateDTO)
        {
            try
            {

                var userExist = await _repository.GetUserByIdAsync(userCreateDTO.EmployeeId);

                if (userExist != null)
                {
                    return ResponseHelper.ErrorResponse<object>($"The employee ID already exists");
                }

                var branchAccesses = userCreateDTO.BranchAccesses
                    .Select(ba => new BranchAccess
                    {
                        BranchId = ba.BranchId,
                        EmployeeId = userCreateDTO.EmployeeId
                    })
                    .ToList();

                var user = _mapper.Map<User>(userCreateDTO);
                user.BranchAccesses = branchAccesses;

                await _repository.AddAsync(user);

                return ResponseHelper.SuccessResponse<object>(null, "User added successfully");
            }
            catch (Exception ex)
            {

                return ResponseHelper.ErrorResponse<object>($"An error occurred while adding the group: {ex.InnerException.Message}");
            }
        }
        public async Task<GenericResponse<UserActiveDirectoryDTO>> CheckUserNameAsync(string username)
        {


            try
            {
                var result = new UserActiveDirectoryDTO();
                using var principalContext = new PrincipalContext(ContextType.Domain, "BOC");
                var domainUser = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, username);

                result.IsExisting = await _repository.IsUserExistingAsync(username);

                if (domainUser != null)
                {
                    result.IsValidBankComUser = true;
                    return ResponseHelper.SuccessResponse<UserActiveDirectoryDTO>(result, "Active Diretory Username exists.");
                }
                else
                {
                    result.IsValidBankComUser = false;
                    return ResponseHelper.FailResponse<UserActiveDirectoryDTO>(result, "Active Diretory Username is not exists.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<UserActiveDirectoryDTO>($"An error occurred while adding the group: {ex.Message}");
            }



        }

        public async Task<GenericResponse<object>> UpdateUserAsync(UserUpdateDTO userUpdateDTO)
        {
            try
            {
                // Fetch the existing user entity
                var existingUser = await _repository.GetUserByIdAsync(userUpdateDTO.EmployeeId);

                if (existingUser == null)
                {
                    return ResponseHelper.ErrorResponse<object>("User not found.");
                }

                // Map DTO to the existing user entity (AutoMapper will update the properties)
                _mapper.Map(userUpdateDTO, existingUser);

                // Update BranchAccessIds explicitly
                var updatedBranchAccesses = userUpdateDTO.BranchAccessIds
                    .Select(branchId => new BranchAccess
                    {
                        UsersLoginName = existingUser.LoginName,
                        BranchId = branchId,
                        EmployeeId = existingUser.EmployeeId
                        
                    }).ToList();

                // Update user and handle BranchAccess changes
                await _repository.UpdateUserAsync(existingUser, updatedBranchAccesses, userUpdateDTO.BranchAccessIds);

                return ResponseHelper.SuccessResponse<object>(null, "User updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<object>($"An error occurred while updating the user: {ex.Message}");
            }
        }

        public async Task<GenericResponse<object>> DeleteUserAsync(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                return ResponseHelper.ErrorResponse<object>("Employee ID cannot be null or empty.");

            try
            {
                await _repository.DeleteUserAsync(employeeId);

                return ResponseHelper.SuccessResponse<object>(null, "User deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<object>($"An error occurred while deleting the user: {ex.Message}");
            }
        }
    }
}
