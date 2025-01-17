using Application.Models.DTOs.Common;
using Application.Models.DTOs.Deviation;
using Application.Models.DTOs.ExceptionsMgmt;
using Application.Models.Responses;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IExceptionsMgmtRepository: IBaseRepository<ExceptionItem>
    {
        Task<List<ExceptionItem>> GetExceptionsList(int? pageNumber = 1, int? pageSize = 10, string? searchString = null, int? status = null);
        Task<ExceptionViewDTO> GetExceptionDetails(string id);
        Task<ExceptionViewDTO> GetExceptionUpdateDetails(string id);
        Task<string> SaveException(ExceptionViewDTO value);
        Task<string> UpdateException(ExceptionViewDTO value);
        Task<List<ExceptionItemRev>> GetExceptionsForApprovalList(int? pageNumber = 1, int? pageSize = 10, string? searchTerm = null, string? currentFilter = null);
        Task<ExceptionViewDTO> GetExceptionsForApprovalDetails(string id);

        Task<List<Deviation>> GetDeviationByClasification(string classification);
        Task<List<Deviation>> GetDeviationByIdsAsync(int[] ids);

        Task<string> DeleteException(string refNo, string? remarks);
        Task<string> ApproveException(string refNo, string? remarks);
        Task<string> RejectException(string refNo, string? remarks);

    }
}
