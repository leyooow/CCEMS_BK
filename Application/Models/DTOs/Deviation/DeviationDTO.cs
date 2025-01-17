using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Deviation
{
    public class DeviationDTO
    {
        public int Id { get; set; }

        public string? Classification { get; set; }

        public string? Category { get; set; }

        public string? Deviation1 { get; set; }

        public string? RiskClassification { get; set; }
    }


}
