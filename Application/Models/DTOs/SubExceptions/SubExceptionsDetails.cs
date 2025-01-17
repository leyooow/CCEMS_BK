using Application.Models.DTOs.ExceptionsMgmt;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.SubExceptions
{
    public class SubExceptionsDetailsDTO
    {
        public ExceptionCode? ExceptionCode { get; set; }
        public DeviationStatusDTO NewStatus { get; set; }
        public string? Remarks { get; set; }

        public string? RiskClassification { get; set; }

        public string? DeviationCategory { get; set; }

        public DateTime? TaggingDate { get; set; }

        public List<string>? BranchReplyDetails { get; set; }

        public int ForDeletion { get; set; }
    }
}
