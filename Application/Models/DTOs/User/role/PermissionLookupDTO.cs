using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.User.role
{
    public class PermissionLookupDTO
    {
        public int Id { get; set; }

        public string? Module { get; set; }

        public string? Function { get; set; }
    }
}
