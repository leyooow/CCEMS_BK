using Application.Contracts.Services;
using Application.Models.DTOs.Report;
using Application.Models.Helpers;
using Application.Models.Responses;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportBranchReplyController : ControllerBase
    {
        private readonly IReportBranchReplyService _service;
        public ReportBranchReplyController(IReportBranchReplyService service)
        {
            _service = service;
        }
        [HttpGet("GetReportContentList")]
        public async Task<IActionResult> GetReportContentList(string id, string refNo, int page = 1) 
        {
            try 
            { 
                return Ok(await _service.GetReportContentList(id, refNo, page));
            } 
            catch 
            {
                return BadRequest();
            }
        }
        [HttpGet("GetBranchReplyList")]
        public async Task<IActionResult> GetBranchReplyList(string id) 
        {
            try 
            { 
                return Ok(await _service.GetBranchReplyList(id));
            } 
            catch 
            {
                return BadRequest();
            }
        }
        [HttpPost("PostReply")]
        public async Task<IActionResult> PostReply(BranchReplyViewModel vm) 
        {
            try 
            { 
                return Ok(await _service.PostReply(vm));
            } 
            catch 
            {
                return BadRequest();
            }
        }
    }
}
