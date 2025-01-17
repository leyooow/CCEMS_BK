using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.FEntities
{
    public class GetBankBusinessDays
    {
        public string CDate { get; set; }    // Formatted date (MM-dd-yyyy)
        public string CWDay { get; set; }   // Day of the week
        public string CMonth { get; set; }  // Extracted month from MMYYYY
        public string BusDay { get; set; }  // Business day indicator (e.g., "Y" or "N")
        public decimal CBrNbr { get; set; } // Numeric branch number

    }
}
