using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.User.user
{
    public class UserActiveDirectoryDTO
    {
        public bool IsValidBankComUser { get; set; }
        public bool IsExisting { get; set; }
        //public List<int> SelectedBranches { get; set; }
    }
}
