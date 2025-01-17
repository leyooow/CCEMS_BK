using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Common
{
    public class UserClaimsDTO
    {
        public string? EmployeeID { get; set; }
        public int? RoleID { get; set; }
        public string? RoleName { get; set; }
        public string Name { get; set; }
        public string LoginName { get; set; }
        public string? LoginDateTime { get; set; }
    }
}
