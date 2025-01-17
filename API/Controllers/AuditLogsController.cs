using Application.Contracts.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet("GetPaginatedLogs")]
        public async Task<IActionResult> GetPaginated([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10, [FromQuery] string? searchTerm = null)
        {

            var response = await _auditLogService.GetPaginatedAsync(pageNumber, pageSize, searchTerm);
            return Ok(response);
        }
    }
}
