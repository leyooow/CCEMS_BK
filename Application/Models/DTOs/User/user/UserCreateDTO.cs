using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.User.user
{
    public class UserCreateDTO
    {

        public string LoginName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int RoleId { get; set; }
        public ICollection<BranchAccess> BranchAccesses { get; set; } = [];
        //public virtual Role Role { get; set; } = null!;
    }
}
