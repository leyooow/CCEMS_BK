using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Common
{
    public class AuditLogsDTO
    {

        
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string ActionType { get; set; }
        public string ActionDesc { get; set; }
        public string ActionBy { get; set; }
        public DateTime DateEntry { get; set; }

    }
}
