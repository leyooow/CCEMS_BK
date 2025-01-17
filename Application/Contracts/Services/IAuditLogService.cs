using Application.Models.DTOs.Common;
using Application.Models.DTOs.User;
using Application.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Services
{
    public interface IAuditLogService
    {
        Task<GenericResponse<PagedResult<AuditLogsDTO>>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm);
    }
}
