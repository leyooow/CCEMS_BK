using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.ExceptionsMgmt
{
    public class ActionPlansDTO
    {
        public Guid Id { get; set; }
        ExceptionItemDTO ExceptionItem { get; set; }
        public string? ExceptionItemRefNo { get; set; }
        //Report Report { get; set; }
        public int ReportId { get; set; }

        public string? CreatedBy { get; set; }
        public string? ActionPlan { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Type { get; set; }
    }
}
