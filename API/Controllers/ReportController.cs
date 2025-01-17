using Application.Contracts.Services;
using Application.Models.DTOs.Report;
using Application.Models.Helpers;
using Application.Models.Responses;
using Application.Models;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        public readonly IReportService _service;
        public ReportController(IReportService service)
        {
            _service = service;
        }
        [HttpGet("GetList")]
        public async Task<PaginatedList<ReportDTO>> GetList(string searchString = "", int Page = 1)
        {
            return await _service.GetList(searchString, Page);
        }
        [HttpGet("PopulateGroupsDropDownList")]
        public async Task<List<Group>> PopulateGroupsDropDownList()
        {
            return await _service.PopulateGroupsDropDownList();
        }
        [HttpPost("DownloadAdhoc")]
        public async Task<IActionResult> DownloadAdhoc(DownloadAdhocViewModel vm)
        {
            return Ok(await _service.DownloadAdhoc(vm));
        }
    }
}
