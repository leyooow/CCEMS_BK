using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Monetary
{
    public class NonMonetaryRevsDTO
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Non-Monetary Category")]
        public NonMonetaryTypes? Type { get; set; }

        [Required]
        [Display(Name = "CIF Number")]
        public string CIFNumber { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Account Number")]
        [RegularExpression("([+-]?[0-9]+(?:\\.[0-9]*)?)", ErrorMessage = "Only allows numeric values.")]
        public string CustomerAccountNo { get; set; }

        public Guid ExceptionID { get; set; }

        public string RefNo { get; set; }
    }
}
