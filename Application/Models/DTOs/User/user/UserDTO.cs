using Application.Models.DTOs.User.role;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.User.user
{
    public class UserDTO
    {
        [Required]
        public string LoginName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public DateTime? LastLogIn { get; set; }
        public string? UserGroup { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Status { get; set; }
        public string? StatusText { get; set; }
        public string? Password { get; set; }
        public int IsLoggedIn { get; set; }
        public int LogInCounter { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Remarks { get; set; }
        public string? IpAddress { get; set; }
        public string? TempIpAddress { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        public ICollection<BranchAccess> BranchAccesses { get; set; } = [];
        public RoleDTO Role { get; set; } = new();
    }

}
