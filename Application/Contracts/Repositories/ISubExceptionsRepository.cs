using Application.Models.DTOs.ExceptionsMgmt;
using Application.Models.DTOs.SubExceptions;
using Application.Models.Responses;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface ISubExceptionsRepository : IBaseRepository<ExceptionCodeRev>
    {
        Task<List<ExceptionCodeRev>> GetSubExceptionsLists(int? pageNumber = 1, int? pageSize = 10, string? searchString = null, string? currentFilter = null);
        Task<List<ExceptionCodeRev>> GetSubExceptionsForApprovalList(int? pageNumber = 1, int? pageSize = 10, string? searchTerm = null, string? currentFilter = null);
        Task<SubExceptionsDetailsDTO> GetSubExceptionDetails(string subERN);
        Task<GenericResponse<dynamic>> DeleteSubException(string subRefNo, string deleteSubExceptionRemarks);
        Task<GenericResponse<dynamic>> UpdateSubException(string subRefNo, DeviationStatusDTO NewStatus, DateTime? TaggingDate, string ExItemRefNo);
        Task<GenericResponse<dynamic>> ApproveSubException(SubExceptionsDetailsDTO value);
        Task<GenericResponse<dynamic>> RejectSubException(SubExceptionsDetailsDTO value, string? remarks);
    }
}
