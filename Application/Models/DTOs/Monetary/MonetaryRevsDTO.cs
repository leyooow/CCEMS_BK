using Application.Models.DTOs.ExceptionsMgmt;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Monetary
{
    public class MonetaryRevsDTO
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sequence No")]
        public string SequenceNo { get; set; }

        [Required]
        [StringLength(4)]
        [Display(Name = "BDS User ID")]
        public string BDSTellerID { get; set; }

        [Required]
        [RegularExpression("([+-]?[0-9]+(?:\\.[0-9]*)?)", ErrorMessage = "Only allows numeric values.")]
        //[Range(0, 999999999)]
        public decimal Amount { get; set; }

        public string TransCode { get; set; }

        public string TransDescription { get; set; }

        [RegularExpression("([+-]?[0-9]+(?:\\.[0-9]*)?)", ErrorMessage = "Only allows numeric values.")]
        public string CreditAccountNo { get; set; }

        public string CreditAccountName { get; set; }
        [RegularExpression("([+-]?[0-9]+(?:\\.[0-9]*)?)", ErrorMessage = "Only allows numeric values.")]
        public string DebitAccountNo { get; set; }

        public string DebitAccountName { get; set; }

        public Guid ExceptionID { get; set; }

        public string RefNo { get; set; }

        public CurrencyDTO Currency { get; set; }
    }
}
