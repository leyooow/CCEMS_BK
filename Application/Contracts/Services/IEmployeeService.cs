using Application.Models.DTOs.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.DTOs.Employee;
using Application.Models.DTOs.Common;
using Application.Models.Responses;
using Infrastructure.Entities;

namespace Application.Contracts.Services
{
    public interface IEmployeeService
    {
        Task<GenericResponse<List<EmployeeDTO>>> GetAllAsync();
        Task<GenericResponse<EmployeeDTO>> GetEmployeeById(string employeeId);
        Task<GenericResponse<PagedResult<EmployeeDTO>>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm);

        Task<GenericResponse<object>> AddEmployeeAsync(EmployeeCreateDTO employeeCreateDto);
    }
}
