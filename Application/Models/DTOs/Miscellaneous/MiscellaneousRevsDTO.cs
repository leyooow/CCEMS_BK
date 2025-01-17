using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Miscellaneous
{
    public class MiscellaneousRevsDTO
    {
        public int Id { get; set; }
        public Guid ExceptionID { get; set; }

        [Required]
        [Display(Name = "Miscellaneous Category")]
        public MiscTypes? Type { get; set; }

        // EMV
        [StringLength(25)]
        [Display(Name = "Card Number")]
        public string CardNo { get; set; }
        //
        //Bank Certification
        [StringLength(25)]
        [Display(Name = "Bank Cert Number")]
        public string BankCertNo { get; set; }
        //
        //General Ledger
        [StringLength(50)]
        [Display(Name = "GL/SL Account Number")]
        public string GLSLAccountNo { get; set; }
        [StringLength(50)]
        [Display(Name = "GL/SL Account Name")]
        public string GLSLAccountName { get; set; }
        //
        //Deposit Pickup Auth Form
        [StringLength(20)]
        [Display(Name = "DPAF Number")]
        public string DPAFNo { get; set; }
        //
        //Other Clearing Deficiencies
        [StringLength(20)]
        [Display(Name = "Check Number")]
        public string CheckNo { get; set; }

        [RegularExpression("([+-]?[0-9]+(?:\\.[0-9]*)?)", ErrorMessage = "Only allows numeric values.")]
        public decimal Amount { get; set; }

        public string RefNo { get; set; }
    }
}
