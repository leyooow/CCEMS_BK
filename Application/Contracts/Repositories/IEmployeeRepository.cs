using Application.Models.DTOs.Group;
using Application.Models.Responses;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        //Task<List<Employee>> GetAllAsync();
        Task<List<Employee>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm);

        Task<Employee> GetEmployeeById(string employeeId);

        Task<Boolean> EmployeeExist(string EmployeeID);



    }
}
