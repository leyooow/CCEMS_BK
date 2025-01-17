using Application.Models.DTOs.User.role;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.User.user
{
    public class UserUpdateDTO
    {
        public string? EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public int Status { get; set; }
        public string? StatusText { get; set; }
        public string? Remarks { get; set; }
        public int RoleId { get; set; }
        public string? UserGroup { get; set; }
        public string LoginName { get; set; } = null!;
        public List<int> BranchAccessIds { get; set; } = [];
        public List<BranchAccessDTO> BranchAccesses { get; set; } = new();

    }
}
