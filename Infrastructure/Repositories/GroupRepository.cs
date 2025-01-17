using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs;
using Application.Models.DTOs.Common;
using Application.Models.Helpers;
using Application.Services.Application.Services;
using AutoMapper;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        private readonly CcemQatContext _context;
        private Logs _auditlogs;
        private readonly IUserClaimsService _userClaimsService;
        private string UserLoginName;


//         public GroupRepository(CcemQatContext context, Logs auditLogs, UserClaimsService userClaimsService) : base(context)

        public GroupRepository(CcemQatContext context,Logs auditLogs, IUserClaimsService userClaimsService) : base(context) 

        {
            _context = context;
            _auditlogs = auditLogs;
            _userClaimsService = userClaimsService;
            UserLoginName = _userClaimsService.GetClaims().LoginName;

        }
        public new async Task AddAsync(Group group)
        {
            group.CreatedBy = UserLoginName;
            AuditLog auditlogs = _auditlogs.SaveLog("Group",
                       "Create",
                       string.Format("Created Group - [Branch Code: {0} | Name: {1} | Area: {2} | Division: {3}]", group.Code, group.Name, group.Area, group.Division),
                         UserLoginName);
            _context.Add(auditlogs);

            group.DateCreated = DateTime.UtcNow;
            await base.AddAsync(group);
        }
        public new async Task UpdateAsync(Group group)
        {
            var existingGroup = await base.GetByIdAsync(group.Id);
            if (existingGroup == null)
            {
                throw new KeyNotFoundException("Group not found.");
            }


            existingGroup.Name = group.Name;
            existingGroup.Name = group.Name;
            existingGroup.Code = group.Code;
            existingGroup.Area = group.Area;
            existingGroup.Division = group.Division;
            existingGroup.DateModified = DateTime.UtcNow;

            AuditLog auditlogs = _auditlogs.SaveLog("Group",
                               "Update",
                               string.Format("Updated Group ID - {0} [Branch Code: {1} | Branch Name: {2} | Area: {3} | Division: {4}]", group.Id, group.Code, group.Name, group.Area, group.Division),
                               UserLoginName);
            _context.Add(auditlogs);
            await _context.SaveChangesAsync();

            await base.UpdateAsync(existingGroup);
        }
        public new async Task DeleteAsync(int id)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id);
            if (group != null)
            {
                AuditLog auditlogs = _auditlogs.SaveLog("Group",
                           "Delete",
                           string.Format("Deleted Group ID - {0} [Branch Code: {1} | Name: {2} | Area: {3} | Division: {4}]", id, group.Code, group.Name, group.Area, group.Division),
                           UserLoginName);

                _context.Add(auditlogs);

                await base.DeleteAsync(id);
            }
            else
            {
                return;
            }

        }
        public async Task<List<Group>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }
        public async Task<List<string>> GetCurrentUserBranchIds()
        {
            var employeeId = _userClaimsService.GetClaims().EmployeeID;

            var result = await _context.BranchAccesses
                   .Where(b => b.EmployeeId == employeeId)
                   .OrderBy(b => b.BranchId)
                   .Select(b => b.BranchId.ToString())
                   .Distinct()
                   .ToListAsync();


            return result;

        }
        public async Task<List<Group>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm)
        {
            IQueryable<Group> query = _context.Set<Group>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(g =>
                    g.Name.Contains(searchTerm) ||
                    g.Code.Contains(searchTerm) ||
                    g.Description.Contains(searchTerm) ||
                    g.Area.Contains(searchTerm) ||
                    g.Division.Contains(searchTerm));
            }

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                query = query
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }
        public new async Task<Group?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<Group?> GetGroupByCode(string? code)
        {
            return await _context.Groups.Where(g => g.Code == code).FirstOrDefaultAsync();
        }

        public async Task<int> GetTotalCountAsync(string? searchTerm)
        {

            return await base.GetTotalCountAsync(searchTerm);
        }

        public async Task<bool> CheckExistingGroupNameAsync(string? name)
        {
            return _context.Groups.Any(g => g.Name.Contains(name));
        }

    }
}
