using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Group
{
    public class GroupUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Area { get; set; }
        public string Division { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
