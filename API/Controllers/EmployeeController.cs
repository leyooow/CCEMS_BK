using Application.Contracts.Services;
using Application.Models.DTOs.Employee;
using Application.Models.DTOs.Group;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }


        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _employeeService.GetAllAsync();
            return Ok(response);
        }
        [HttpGet("GetEmployeeById/{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(string employeeId)
        {
            var response = await _employeeService.GetEmployeeById(employeeId);
            return Ok(response);
        }


        [HttpGet("GetPaginatedEmployees")]
        public async Task<IActionResult> GetPaginated([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            var response = await _employeeService.GetPaginatedAsync(pageNumber, pageSize, searchTerm);
            return Ok(response);
        }

        [HttpPost("CreateEmployee")]
        public async Task<IActionResult> Create(EmployeeCreateDTO employeeCreateDto)
        {
            var response = await _employeeService.AddEmployeeAsync(employeeCreateDto);
            return Ok(response);
        }

    }


}
