using Application.Contracts.Services;
using Application.Models.DTOs.ExceptionsMgmt;
using Application.Models.DTOs.Group;
using Application.Models.DTOs.SubExceptions;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExceptionsMgmtController : ControllerBase
    {
        private readonly IExceptionsMgmtService _exceptionsMgmtService;
        public ExceptionsMgmtController(IExceptionsMgmtService exceptionsMgmtService)
        {
            _exceptionsMgmtService = exceptionsMgmtService;
        }

        [HttpGet("GetExceptionsList")]
        public async Task<IActionResult> GetExceptionsList([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] int? status = null)
        {
            var response = await _exceptionsMgmtService.GetExceptionsList(pageNumber, pageSize, searchTerm, status);
            return Ok(response);
        }

        [HttpGet("GetExceptionDetails")]
        public async Task<IActionResult> GetExceptionDetails([FromQuery] string id)
         {
            var response = await _exceptionsMgmtService.GetExceptionDetails(id);
            return Ok(response);
        }

        [HttpPost("SaveException")]
        public async Task<IActionResult> SaveException(ExceptionViewDTO value)
        {
            var response = await _exceptionsMgmtService.SaveException(value);
            return Ok(response);
        }

        [HttpGet("GetExceptionUpdateDetails")]
        public async Task<IActionResult> GetExceptionUpdateDetails([FromQuery] string id)
        {
            var response = await _exceptionsMgmtService.GetExceptionUpdateDetails(id);
            return Ok(response);
        }

        [HttpPut("UpdateException")]
        public async Task<IActionResult> Update(ExceptionViewDTO value)
        {
            var response = await _exceptionsMgmtService.UpdateException(value);
            return Ok(response);
        }

        [HttpGet("GetExceptionsForApprovalList")]
        public async Task<IActionResult> GetExceptionsForApprovalList([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10, [FromQuery] string? searchString = null, [FromQuery] string? currentFilter = null)
        {
            var response = await _exceptionsMgmtService.GetExceptionsForApprovalList(pageNumber, pageSize, searchString, currentFilter);
            return Ok(response);
        }

        [HttpGet("GetExceptionsForApprovalDetails")]
        public async Task<IActionResult> GetExceptionsForApprovalDetails(string id)
        {
            var response = await _exceptionsMgmtService.GetExceptionsForApprovalDetails(id);
            return Ok(response);
        }

        [HttpGet("GetDeviationByClasification/{classification}")]
        public async Task<IActionResult> GetDeviationByClasification(string classification)
        {
            var response = await _exceptionsMgmtService.GetDeviationByClasification(classification);
            return Ok(response);
        }


        [HttpGet("GetDeviationByIds")]
        public async Task<IActionResult> GetDeviationsByIds([FromQuery] int[] ids)
        {
            var deviations = await _exceptionsMgmtService.GetDeviationByIdsAsync(ids);
            return Ok(deviations);
        }


        [HttpDelete("DeleteException/{refNo}")]
        public async Task<IActionResult> DeleteException(string refNo, string? remarks)
        {
            var response = await _exceptionsMgmtService.DeleteException(refNo, remarks);
            return Ok(response);
        }

        [HttpPut("ApproveException/{refNo}")]
        public async Task<IActionResult> ApproveException(string refNo, string remarks)
        {
            var response = await _exceptionsMgmtService.ApproveException(refNo, remarks);
            return Ok(response);
        }

        [HttpPut("RejectException/{refNo}")]
        public async Task<IActionResult> RejectException(string refNo, string remarks)
        {
            var response = await _exceptionsMgmtService.RejectException(refNo, remarks);
            return Ok(response);
        }
    }
}