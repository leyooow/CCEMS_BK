using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Group;
using Application.Models.Helpers;
using Application.Models.Responses;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repository;
        private readonly IMapper _mapper;

        public AuditLogService(IAuditLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }
        public async Task<GenericResponse<PagedResult<AuditLogsDTO>>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm)
        {
            try
            {
                var auditLogs = await _repository.GetPaginatedAsync(pageNumber, pageSize, searchTerm);
                var auditLogsDTOs = _mapper.Map<List<AuditLogsDTO>>(auditLogs);

                // Get the total count of groups for pagination metadata
                var totalCount = await _repository.GetTotalCountAsync(searchTerm);

                var pagedResult = new PagedResult<AuditLogsDTO>
                {
                    Items = auditLogsDTOs,
                    TotalCount = totalCount,
                    PageNumber = pageNumber ?? 1,  // Default to 1 if not provided
                    PageSize = pageSize ?? 10      // Default to 10 if not provided
                };

                return ResponseHelper.SuccessResponse(pagedResult, "Paginated Logs retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<PagedResult<AuditLogsDTO>>($"Failed to retrieve logs: {ex.Message}");
            }
        }
    }
}
