using Application.Contracts.Repositories;
using Application.Models.DTOs.User.role;
using Application.Models.Helpers;
using Application.Services.Application.Services;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

using System;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices.AccountManagement;
using Application.Models.DTOs.User.role;
using Application.Contracts.Services;



namespace Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly CcemQatContext _context;
        private Logs _auditlogs;
        private readonly IUserClaimsService _userClaimsService;
        private string UserLoginName;

        public UserRepository(CcemQatContext context, Logs auditLogs, IUserClaimsService userClaimsService) : base(context)
        {
            _context = context;
            _auditlogs = auditLogs;
            _userClaimsService = userClaimsService;
            UserLoginName = _userClaimsService.GetClaims().LoginName;
        }
        public async Task<List<User>> GetPaginatedAsync(int? pageNumber, int? pageSize, string? searchTerm)
        {
            IQueryable<User> query = _context.Users
                .Include(u => u.BranchAccesses);
                //.Include(u => u.Role)
              

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(g =>
                    g.EmployeeId.Contains(searchTerm) ||
                    g.LoginName.Contains(searchTerm) ||
                    g.CreatedDate.ToString().Contains(searchTerm) ||
                    g.Role.RoleName.Contains(searchTerm)
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
        public async Task<List<User>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }
        public async Task<User?> GetUserByIdAsync(string? id)
        {
            var entity = await _context.Users
                .Include(x => x.BranchAccesses)
                .Where(x => x.EmployeeId == id)
                .FirstOrDefaultAsync();

            return entity;
        }
        public async Task<List<Role>> GetAllRolesAsync()
        {
            var entity = await _context.Roles.ToListAsync();

            return entity;
        }
        public async Task<List<PermissionLookup>> GetAllPermissionLookUpAsync()
        {
            var entity = await _context.PermissionLookups.ToListAsync();

            return entity;
        }
        public async Task<List<RolePermission>> GetPermissionsByRoleId(int roleId)
        {
            return await _context.RolePermissions
                .Where(p => p.RoleId == roleId)
                .ToListAsync();
        }
        public async Task AddPermissionsAsync(AddPermissionRequest addPermissionRequest)
        {

            // Delete existing role permissions
            var existingPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == addPermissionRequest.RoleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(existingPermissions);



            var rolePermissionEntities = addPermissionRequest.PermissionList.Select(permission => new RolePermission
            {
                RoleId = addPermissionRequest.RoleId,
                Permission = permission
            }).ToList();

            await _context.RolePermissions.AddRangeAsync(rolePermissionEntities);
            await _context.SaveChangesAsync();

        }
        public new async Task AddAsync(User user)
        {
            var auditMessage = $"Created User - [Login Name: {user.LoginName} | Employee ID: {user.EmployeeId} | Full Name: {user.LastName}, {user.FirstName} {user.MiddleName} | Email: {user.Email} | Role ID: {user.RoleId} | Group ID: {string.Join(", ", user.BranchAccesses)}]";

            var auditLog = _auditlogs.SaveLog("Users", "Create", auditMessage, UserLoginName);
            _context.Add(auditLog);

            user.CreatedDate = DateTime.UtcNow;
            user.IsLoggedIn = 0;

            await base.AddAsync(user);
        }

        public async Task<bool> IsUserExistingAsync(string username)
        {
            return await _context.Users.AnyAsync(s => s.LoginName == username);
        }

        public new async Task UpdateAsync(User user)
        {


            await base.UpdateAsync(user);
        }

        public async Task UpdateUserAsync(User user, List<BranchAccess> updatedBranchAccesses, List<int> branchAccessIds)
        {
            
            var existingUser = await _context.Users
                .Include(u => u.BranchAccesses)
                .FirstOrDefaultAsync(u => u.EmployeeId == user.EmployeeId);

            if (existingUser == null)
                throw new InvalidOperationException("User not found");

            // Update user scalar properties
            _context.Entry(existingUser).CurrentValues.SetValues(user);

            // Update BranchAccesses
            var currentBranchAccesses = existingUser.BranchAccesses.ToList();

            // Remove branches not in the updated list
            var branchesToRemove = currentBranchAccesses
                .Where(b => !updatedBranchAccesses.Any(ub => ub.BranchId == b.BranchId))
                .ToList();

            // Add new branches
            var branchesToAdd = updatedBranchAccesses
                .Where(ub => !currentBranchAccesses.Any(cb => cb.BranchId == ub.BranchId))
                .ToList();

            // Apply changes
            _context.BranchAccesses.RemoveRange(branchesToRemove);
            await _context.BranchAccesses.AddRangeAsync(branchesToAdd);

            // Save changes
            await _context.SaveChangesAsync();

            // Create audit log
            var auditMessage = $"Updated User - [Login Name: {existingUser.LoginName} | Employee ID: {existingUser.EmployeeId} | Full Name: {existingUser.LastName}, {existingUser.FirstName} {existingUser.MiddleName} | Email: {existingUser.Email} | Role ID: {existingUser.RoleId} | Branch IDs: {string.Join(", ", branchAccessIds)}]";

            var auditLog = _auditlogs.SaveLog("Users", "Update", auditMessage, UserLoginName);

            // Add the audit log to the context
            await _context.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string employeeId)
        {
            try
            {

                var users = await _context.Users.Include(b => b.BranchAccesses).FirstOrDefaultAsync(m => m.EmployeeId == employeeId);

                _context.Users.Remove(users);
                await _context.SaveChangesAsync();
                string branchIDs = AppendBranchIDs(users);

                AuditLog auditlogs = _auditlogs.SaveLog("Users",
                        "Delete",
                        string.Format("Deleted User ID - {0} [Employee ID: {1} | Full Name: {2}, {3} {4} | Email: {5} | Role ID: {6} | Group ID: {7} | Remarks: {8}]", users.LoginName, users.EmployeeId, users.LastName, users.FirstName, users.MiddleName, users.Email, users.RoleId, branchIDs, users.Remarks),
                        UserLoginName);
                _context.Add(auditlogs);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex){
                return;
            }
        }

        //public async Task<User> GetUserWithBranchAccessesAsync(string employeeId
        //{
             
        //    return await _context.Users
        //        .Include(u => u.BranchAccesses)
        //        .FirstOrDefaultAsync(u => u.EmployeeId == employeeId); 
        //}

        private static string AppendBranchIDs(User user)
        {
            string branchIDs = string.Empty;
            foreach (var item in user.BranchAccesses)
            {
                if (branchIDs != string.Empty)
                    branchIDs += ("," + item.BranchId);
                else
                    branchIDs += item.BranchId;
            }

            return branchIDs;
        }

    }

}
