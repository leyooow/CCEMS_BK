using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Group
{
    public class GroupCreateDTO
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public string? CreatedBy { get; set; }
        public string? Area { get; set; }
        public string? Division { get; set; }
    }
}
