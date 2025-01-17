using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Group;
using Application.Models.DTOs.User;
using Application.Models.Helpers;
using Application.Models.Responses;
using AutoMapper;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _repository;
        private readonly IMapper _mapper;

        public GroupService(IGroupRepository repository, IMapper mapper, Logs auditlogs)
        {
            _repository = repository;
            _mapper = mapper;
           
        }

        public async Task<GenericResponse<List<GroupDTO>>> GetAllAsync() 
        {
            var groups = await _repository.GetAllAsync();

            var groupDtos = _mapper.Map<List<GroupDTO>>(groups);

           return ResponseHelper.SuccessResponse(groupDtos,"Groups retrieved succesfully.");
        }        


        public async Task<GenericResponse<PagedResult<GroupDTO>>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm)
        {
            try
            {
                var groups = await _repository.GetPaginatedAsync(pageNumber, pageSize, searchTerm);
                var groupDtos = _mapper.Map<List<GroupDTO>>(groups);

                // Get the total count of groups for pagination metadata
                var totalCount = await _repository.GetTotalCountAsync(searchTerm);

                var pagedResult =new  PagedResult<GroupDTO>
                {
                    Items = groupDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber ?? 1,  // Default to 1 if not provided
                    PageSize = pageSize ?? 10      // Default to 10 if not provided
                };
                
                return ResponseHelper.SuccessResponse(pagedResult, "Paginated Groups retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<PagedResult<GroupDTO>>($"Failed to retrieve groups: {ex.Message}");
            }
        }


        public async Task <GenericResponse<GroupDTO?>> GetGroupByIdAsync(int id)
        {
            try
            {
                var group = await _repository.GetByIdAsync(id);
                var mapgroup = group == null ? null : _mapper.Map<GroupDTO>(group);

                return ResponseHelper.SuccessResponse(mapgroup, "Group retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<GroupDTO?>("An error occurred while retrieving the group.");
            }

        }

        public async Task<GenericResponse<GroupDTO?>> GetGroupByCodeAsync(string code)
        {
            try
            {
                var group = await _repository.GetGroupByCode(code);
                var mapgroup = group == null ? null : _mapper.Map<GroupDTO>(group);

                return ResponseHelper.SuccessResponse(mapgroup, "Group retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<GroupDTO?>("An error occurred while retrieving the group.");
            }

        }



        public async Task<List<GroupDTO>> GetBranchDetailsAsync(IEnumerable<string> branchCodes)
        {
            var branchDetails = new List<GroupDTO>();

            foreach (var code in branchCodes)
            {
                var response = await GetGroupByCodeAsync(code);
                if (response.Success && response.Data != null)
                {
                    branchDetails.Add(response.Data);
                }
            }

            return branchDetails;
        }


        public async Task<List<GroupDTO>> GetBranchDropdown()
        {
            var branchDetails = new List<GroupDTO>();
            var branchCodes = await _repository.GetCurrentUserBranchIds();
            foreach (var code in branchCodes)
            {
                var response = await GetGroupByCodeAsync(code);
                if (response.Success && response.Data != null)
                {
                    branchDetails.Add(response.Data);
                }
            }

            return branchDetails;

        }

        public async Task<GenericResponse<object>> AddGroupAsync(GroupCreateDTO groupCreateDto)
        {
            var checkexisting = await _repository.CheckExistingGroupNameAsync(groupCreateDto.Name);
            if (checkexisting)
            {
                return ResponseHelper.ErrorResponse<object>("Group Name Already exists.");
            }
            else
            {
                try
                {

                    var group = _mapper.Map<Group>(groupCreateDto);

                    await _repository.AddAsync(group);

                    return ResponseHelper.SuccessResponse<object>(null, "Group added successfully");
                }
                catch (Exception ex)
                {
                    // Log the exception if needed
                    return ResponseHelper.ErrorResponse<object>($"An error occurred while adding the group: {ex.Message}");
                }
            }
           

        }

        public async Task <GenericResponse<object>> UpdateGroupAsync(GroupUpdateDTO groupUpdateDto)
        {
            var checkexisting = await _repository.CheckExistingGroupNameAsync(groupUpdateDto.Name);
            if (checkexisting)
            {
                return ResponseHelper.ErrorResponse<object>("Group Name Already exists.");
            }
            else
            {
                try
                {
                    var group = _mapper.Map<Group>(groupUpdateDto);
                    await _repository.UpdateAsync(group);

                    return ResponseHelper.SuccessResponse<object>(null, "Group updated successfully");
                }
                catch (Exception ex)
                {
                    return ResponseHelper.ErrorResponse<object>($"An error occurred while adding the group: {ex.Message}");
                }
            }
           
        }

        public async Task<GenericResponse<object>> DeleteGroupAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return ResponseHelper.SuccessResponse<object>(false, "Group deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<object>($"Failed to delete group: {ex.Message}");
            }


        }

        
    }
}
