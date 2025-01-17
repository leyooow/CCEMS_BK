using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Report
{
    public class SendRequestDTO
    {
        public List<string> ToList { get; set; }
        public List<string> CCList { get; set; }
        public int id { get; set; }
    }
}
