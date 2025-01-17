using Application.Contracts.Services;
using Application.Models.DTOs.ExceptionsMgmt;
using Application.Models.DTOs.SubExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubExceptionsController : ControllerBase
    {
        private readonly ISubExceptionsService _subExceptionsService;
        public SubExceptionsController(ISubExceptionsService subExceptionsService)
        {
            _subExceptionsService = subExceptionsService;
        }

        [HttpGet("GetSubExceptionLists")]
        public async Task<IActionResult> GetSubExceptionLists([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10, [FromQuery] string? searchString = null)
        {
            var response = await _subExceptionsService.GetSubExceptionLists(pageNumber, pageSize, searchString);
            return Ok(response);
        }

        [HttpGet("GetSubExceptionsForApprovalList")]
        public async Task<IActionResult> GetSubExceptionsForApprovalList([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10, [FromQuery] string? searchString = null, [FromQuery] string? currentFilter = null)
        {
            var response = await _subExceptionsService.GetSubExceptionsForApprovalList(pageNumber, pageSize, searchString, currentFilter);
            return Ok(response);
        }

        [HttpGet("GetSubExceptionDetails/{subERN}")]
        public async Task<IActionResult> GetSubExceptionDetails(string subERN)
        {
            var response = await _subExceptionsService.GetSubExceptionDetails(subERN);
            return Ok(response);
        }

        [HttpDelete("DeleteSubException/{subRefNo}")]
        public async Task<IActionResult> DeleteSubException(string subRefNo, [FromQuery] string deleteSubExceptionRemarks)
        {
            var response = await _subExceptionsService.DeleteSubException(subRefNo, deleteSubExceptionRemarks);
            return Ok(response);
        }

        [HttpPut("UpdateSubException/{subRefNo}")]
        public async Task<IActionResult> UpdateSubException(string subRefNo, [FromQuery] DeviationStatusDTO NewStatus, DateTime? TaggingDate, string ExItemRefNo)
        {
            var response = await _subExceptionsService.UpdateSubException(subRefNo, NewStatus, TaggingDate, ExItemRefNo);
            return Ok(response);
        }

        [HttpPost("ApproveSubException")]
        public async Task<IActionResult> ApproveSubException(SubExceptionsDetailsDTO value)
        {
            var response = await _subExceptionsService.ApproveSubException(value);
            return Ok(response);
        }

        [HttpPost("RejectSubException")]
        public async Task<IActionResult> RejectSubException(SubExceptionsDetailsDTO value, string? remarks)
        {
            var response = await _subExceptionsService.RejectSubException(value, remarks);
            return Ok(response);
        }
    }
}
