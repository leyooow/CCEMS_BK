using Application.Models.DTOs.Common;
using Application.Models.DTOs.Deviation;
using Application.Models.DTOs.Employee;
using Application.Models.DTOs.ExceptionsMgmt;
using Application.Models.DTOs.SubExceptions;
using Application.Models.Responses;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Services
{
    public interface IExceptionsMgmtService
    {
        Task<GenericResponse<PagedResult<ExceptionItemDTO>>> GetExceptionsList(int? pageNumber, int? pageSize, string? searchTerm, int? status);
        Task<GenericResponse<ExceptionViewDTO>> GetExceptionDetails(string id);
        Task<GenericResponse<ExceptionViewDTO>> GetExceptionUpdateDetails(string id);
        Task<GenericResponse<object>> SaveException(ExceptionViewDTO value);
        Task<GenericResponse<object>> UpdateException(ExceptionViewDTO value);
        Task<GenericResponse<PagedResult<ExceptionItemRevsDTO>>> GetExceptionsForApprovalList(int? pageNumber = 1, int? pageSize = 10, string? searchString = null, string? currentFilter = null);
        Task<GenericResponse<ExceptionViewDTO>> GetExceptionsForApprovalDetails(string id);
        Task<GenericResponse<List<DeviationDTO>>> GetDeviationByClasification(string classification);
        Task<GenericResponse<List<DeviationDTO>>> GetDeviationByIdsAsync(int[] ids);
        Task<GenericResponse<object>> DeleteException(string subRefNo, string? remarks);
        Task<GenericResponse<object>> ApproveException(string refNo, string? remarks);
        Task<GenericResponse<object>> RejectException(string refNo, string? remarks);

    }
}
