using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Group;
using Application.Models.Helpers;
using Application.Services.Application.Services;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private readonly CcemQatContext _context;
        private Logs _auditlogs;
        private readonly IUserClaimsService _userClaimsService;
        private string UserLoginName;
        public EmployeeRepository(CcemQatContext context, Logs auditLogs, IUserClaimsService userClaimsService) : base(context)
        {
            _context = context;
            _auditlogs = auditLogs;
            _userClaimsService = userClaimsService;
            UserLoginName = _userClaimsService.GetClaims().LoginName;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }
        public async Task<Employee> GetEmployeeById(string employeeId)
        {
            return await _context.Employees.Where(e => e.EmployeeId == employeeId).FirstOrDefaultAsync();
        }
        public async Task AddAsync(Employee employee)
        {
            AuditLog auditlogs = _auditlogs.SaveLog("Users", "AddEmployee",
                              string.Format("New Employee Added - Details: [Employee ID: {0} | First Name: {1} | Middle Name: {2} | Last Name: {3}]", employee.EmployeeId, employee.FirstName, employee.MiddleName, employee.LastName),
                              UserLoginName);
            _context.Add(auditlogs);

            await _context.SaveChangesAsync();

            await base.AddAsync(employee);
        }
        public async Task<List<Employee>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm)
        {
            IQueryable<Employee> query = _context.Set<Employee>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(g => g.FirstName.Contains(searchTerm) ||
                g.MiddleName.Contains(searchTerm) ||
                g.EmployeeId.Contains(searchTerm) ||
                g.LastName.Contains(searchTerm)
                );
            }

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                query = query
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? searchTerm)
        {

            return await base.GetTotalCountAsync(searchTerm);
        }
        public async Task<Boolean> EmployeeExist(string EmployeeID)
        {
            return await _context.Employees.AnyAsync(s => s.EmployeeId == EmployeeID);
        }

    }
}
