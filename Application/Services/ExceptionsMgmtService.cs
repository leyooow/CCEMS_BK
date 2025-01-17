using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Deviation;
using Application.Models.DTOs.Employee;
using Application.Models.DTOs.ExceptionsMgmt;
using Application.Models.Helpers;
using Application.Models.Responses;
using AutoMapper;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ExceptionsMgmtService : IExceptionsMgmtService
    {
        private readonly IExceptionsMgmtRepository _repository;
        private readonly IMapper _mapper;

        public ExceptionsMgmtService(IExceptionsMgmtRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<GenericResponse<PagedResult<ExceptionItemDTO>>> GetExceptionsList(int? pageNumber, int? pageSize, string? searchTerm, int? status)
        {
            try
            {
                var employees = await _repository.GetExceptionsList(pageNumber, pageSize, searchTerm, status);
                var employeeDtos = _mapper.Map<List<ExceptionItemDTO>>(employees);

                // Get the total count of groups for pagination metadata
                var totalCount = await _repository.GetTotalCountAsync(searchTerm);

                var pagedResult = new PagedResult<ExceptionItemDTO>
                {
                    Items = employeeDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber ?? 1,  // Default to 1 if not provided
                    PageSize = pageSize ?? 10      // Default to 10 if not provided
                };

                return ResponseHelper.SuccessResponse(pagedResult, "Paginated Exceptions List retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<PagedResult<ExceptionItemDTO>>($"Failed to retrieve Exceptions List: {ex.Message}");

            }
        }
        public async Task<GenericResponse<ExceptionViewDTO>> GetExceptionDetails(string id)
        {
            try
            {
                var exceptionDetails = await _repository.GetExceptionDetails(id);

                return ResponseHelper.SuccessResponse(exceptionDetails, "Exception Details retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<ExceptionViewDTO>($"Failed to retrieve Exception Details: {ex.Message}");

            }
        }

        public async Task<GenericResponse<ExceptionViewDTO>> GetExceptionUpdateDetails(string id)
        {
            try
            {
                var exceptionDetails = await _repository.GetExceptionUpdateDetails(id);

                return ResponseHelper.SuccessResponse(exceptionDetails, "Exception Update Details retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<ExceptionViewDTO>($"Failed to retrieve Exception Update Details: {ex.Message}");

            }
        }
        public async Task<GenericResponse<object>> SaveException(ExceptionViewDTO value)
        {
            try
            {
                string refNo = await _repository.SaveException(value);
                return ResponseHelper.SuccessResponse<object>(null, string.Format("Exception with Ref No [{0}] has been successfully created.", refNo));
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<object>($"Failed to save Exception: {ex.Message}");

            }
        }
        public async Task<GenericResponse<object>> UpdateException(ExceptionViewDTO value)
        {
            try
            {
                string message = await _repository.UpdateException(value);
                return ResponseHelper.SuccessResponse<object>(null, message);
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<object>($"Failed to save Exception: {ex.Message}");

            }
        }
        public async Task<GenericResponse<PagedResult<ExceptionItemRevsDTO>>> GetExceptionsForApprovalList(int? pageNumber = 1, int? pageSize = 10, string? searchString = null, string? currentFilter = null)
        {
            try
            {
                var list = await _repository.GetExceptionsForApprovalList(pageNumber, pageSize, searchString, currentFilter);
                var listDTOs = _mapper.Map<List<ExceptionItemRevsDTO>>(list);

                // Get the total count of groups for pagination metadata
                var totalCount = await _repository.GetTotalCountAsync(searchString);

                var pagedResult = new PagedResult<ExceptionItemRevsDTO>
                {
                    Items = listDTOs,
                    TotalCount = totalCount,
                    PageNumber = pageNumber ?? 1,  // Default to 1 if not provided
                    PageSize = pageSize ?? 10      // Default to 10 if not provided
                };

                return ResponseHelper.SuccessResponse(pagedResult, "Paginated Exceptions For Approval List retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<PagedResult<ExceptionItemRevsDTO>>($"Failed to retrieve Exceptions For Approval  List: {ex.Message}");

            }
        }

        public async Task<GenericResponse<ExceptionViewDTO>> GetExceptionsForApprovalDetails(string id)
        {
            try
            {
                var exceptionDetails = await _repository.GetExceptionsForApprovalDetails(id);

                return ResponseHelper.SuccessResponse(exceptionDetails, "Exception For Approval Details retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<ExceptionViewDTO>($"Failed to retrieve Exception For Approval Details: {ex.Message}");

            }
        }

        public async Task<GenericResponse<object>> DeleteException(string subRefNo, string? remarks)
        {
            try
            {
                var result = await _repository.DeleteException(subRefNo, remarks);
                return ResponseHelper.SuccessResponse<object>(null, result);
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<object>($"Failed to reject exception: {ex.Message}");

            }
        }
        public async Task<GenericResponse<object>> ApproveException(string refNo, string? remarks)
        {
            try
            {
                var result = await _repository.ApproveException(refNo, remarks);
                return ResponseHelper.SuccessResponse<object>(null, result);
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<object>($"Failed to approve exception: {ex.Message}");

            }
        }

        public async Task<GenericResponse<object>> RejectException(string refNo, string? remarks)
        {
            try
            {
                var result = await _repository.RejectException(refNo, remarks);
                return ResponseHelper.SuccessResponse<object>(null, result);
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<object>($"Failed to reject exception: {ex.Message}");

            }
        }

        public async Task<GenericResponse<List<DeviationDTO>>> GetDeviationByClasification(string classification)
        {
            try
            {
                var deviation = await _repository.GetDeviationByClasification(classification);
                var deviationDTO = _mapper.Map<List<DeviationDTO>>(deviation);


                return ResponseHelper.SuccessResponse(deviationDTO, "Deviation list retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<List<DeviationDTO>>($"Failed to retrieve Deviation List: {ex.Message}");
            }

        }

        public async Task<GenericResponse<List<DeviationDTO>>> GetDeviationByIdsAsync(int[] ids)
        {
            try
            {
                var deviation = await _repository.GetDeviationByIdsAsync(ids);
                var deviationDTO = _mapper.Map<List<DeviationDTO>>(deviation);


                return ResponseHelper.SuccessResponse(deviationDTO, "Deviation list retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<List<DeviationDTO>>($"Failed to retrieve Deviation List: {ex.Message}");
            }
        }
    }
}
