using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Employee;
using Application.Models.DTOs.FBranch;
using Application.Models.DTOs.User.user;
using Application.Models.Helpers;
using Application.Models.Responses;
using AutoMapper;
using Domain.FEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BranchCodeService : IBranchCodeService
    {
        private readonly IBranchCodeRepository _repository;
        private readonly IMapper _mapper;


       
        public BranchCodeService(IBranchCodeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GenericResponse<List<BranchCodeTableDTO>>> GetAllAsync()
        {
            var branchCodes = await _repository.GetAllAsync();

            var branchCodesDtos = _mapper.Map<List<BranchCodeTableDTO>>(branchCodes);

            return ResponseHelper.SuccessResponse(branchCodesDtos, "Branches retrieved succesfully.");
        }

        public async Task<BranchCodeTable> GetByIdAsync(string? code)
      {
            return await _repository.GetByIdAsync(code);
        }

        public async Task<GenericResponse<PagedResult<BranchCodeTableDTO>>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm)
        {
            try
            {
                var branchCodes = await _repository.GetPaginatedAsync(pageNumber, pageSize, searchTerm);
                var branchCodeTableDTOs = _mapper.Map<List<BranchCodeTableDTO>>(branchCodes);
                var totalCount = await _repository.GetTotalCountAsync(searchTerm);

                var pagedResult = new PagedResult<BranchCodeTableDTO>
                {
                    Items = branchCodeTableDTOs,
                    TotalCount = totalCount,
                    PageNumber = pageNumber ?? 1, 
                    PageSize = pageSize ?? 10  
                };

                return ResponseHelper.SuccessResponse(pagedResult, "Paginated Users retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<PagedResult<BranchCodeTableDTO>>($"Failed to retrieve groups: {ex.Message}");
            }
        }

        //public async Task<GenericResponse<PagedResult<UserDTO>>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm)
        //{
        //    try
        //    {
        //        var Users = await _repository.GetPaginatedAsync(pageNumber, pageSize, searchTerm);
        //        var UserDtos = _mapper.Map<List<UserDTO>>(Users);
        //        var userRoles = await _repository.GetAllRolesAsync();

        //        // Map role names to UserDTOs
        //        foreach (var userDto in UserDtos)
        //        {
        //            var userRole = userRoles
        //                .SingleOrDefault(r => r.Id == userDto.RoleId); // Match UserRoleId

        //            userDto.RoleName = userRole?.RoleName ?? "Unknown"; // Handle null case
        //        }
        //        //var branchCount = UserDtos.Select(u => u.BranchAccesses).Count();

        //        // Get the total count of Users for pagination metadata
        //        var totalCount = await _repository.GetTotalCountAsync(searchTerm);

        //        var pagedResult = new PagedResult<UserDTO>
        //        {
        //            Items = UserDtos,
        //            TotalCount = totalCount,
        //            PageNumber = pageNumber ?? 1,  // Default to 1 if not provided
        //            PageSize = pageSize ?? 10      // Default to 10 if not provided
        //        };

        //        return ResponseHelper.SuccessResponse(pagedResult, "Paginated Users retrieved successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return ResponseHelper.ErrorResponse<PagedResult<UserDTO>>($"Failed to retrieve groups: {ex.Message}");
        //    }
        //}
    }
}
