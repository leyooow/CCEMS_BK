using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.User.role
{
    public class BranchAccessDTO
    {
        public int Id { get; set; }

        public string? EmployeeId { get; set; }

        public int BranchId { get; set; }

        public string? UsersLoginName { get; set; }

        //public User UsersLoginNameNavigation { get; set; } = null;
    }
}
