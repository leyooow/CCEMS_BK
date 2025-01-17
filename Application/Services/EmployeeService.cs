using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Employee;
using Application.Models.DTOs.Group;
using Application.Models.Helpers;
using Application.Models.Responses;
using AutoMapper;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GenericResponse<object>> AddEmployeeAsync(EmployeeCreateDTO employeeCreateDto)
        {
            if(await _repository.EmployeeExist(employeeCreateDto.EmployeeId))
                return ResponseHelper.ErrorResponse<object>("Creation of Employee did not push through. Employee ID already exist.");

            var employee = _mapper.Map<Employee>(employeeCreateDto);
            await _repository.AddAsync(employee);

            return ResponseHelper.SuccessResponse<object>(null, "Employee added successfully");

        }

        public async Task<GenericResponse<List<EmployeeDTO>>> GetAllAsync()
        {
            var employees = await _repository.GetAllAsync();

            var employeesDtos = _mapper.Map<List<EmployeeDTO>>(employees);

            return ResponseHelper.SuccessResponse(employeesDtos, "Employees retrieved succesfully.");
        }

        public async Task<GenericResponse<EmployeeDTO>> GetEmployeeById(string employeeId)
        {
            try
            {
                var employee = await _repository.GetEmployeeById(employeeId);

                if (employee == null)
                {
                    return ResponseHelper.ErrorResponse<EmployeeDTO>($"Employee not found.");
                }

                var employeeDtos = _mapper.Map<EmployeeDTO>(employee);
                return ResponseHelper.SuccessResponse<EmployeeDTO>(employeeDtos, "Employee retrieved succesfully.");

            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<EmployeeDTO>($"Failed to retrieve employee: {ex.Message}");

            }

        }
        public async Task<GenericResponse<PagedResult<EmployeeDTO>>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm)
        {
            try
            {

                var employees = await _repository.GetPaginatedAsync(pageNumber, pageSize, searchTerm);
                var employeeDtos = _mapper.Map<List<EmployeeDTO>>(employees);

                // Get the total count of groups for pagination metadata
                var totalCount = await _repository.GetTotalCountAsync(searchTerm);

                var pagedResult = new PagedResult<EmployeeDTO>
                {
                    Items = employeeDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber ?? 1,  // Default to 1 if not provided
                    PageSize = pageSize ?? 10      // Default to 10 if not provided
                };

                return ResponseHelper.SuccessResponse(pagedResult, "Paginated Employees retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<PagedResult<EmployeeDTO>>($"Failed to retrieve employees: {ex.Message}");

            }

        }
    }
}
