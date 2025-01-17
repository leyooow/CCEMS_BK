using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Miscellaneous
{
    public class MiscellaneousDTO
    {
        public int Id { get; set; }
        public MiscTypes? Category { get; set; }

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

        public Guid ExceptionID { get; set; }

        public string RefNo { get; set; }
    }

    public enum MiscTypes
    {
        EMV = 1,
        [Display(Name = "Bank Certification")]
        BankCert = 2,
        [Display(Name = "General Ledger")]
        GeneralLedger = 3,
        [Display(Name = "Deposit Pick-up Authorization Form")]
        DepositPickupAuth = 4,
        [Display(Name = "Due From Local Banks")]
        DueFromLocalBanks = 5,
        [Display(Name = "BDS Reports")]
        BDSReports = 6,
        [Display(Name = "Other Clearing Deficiencies")]
        ClearingDeficiencies = 7,
        [Display(Name = "Checkbook")]
        Checkbook = 8,
        [Display(Name = "New Account Tagging")]
        NewAccountTagging = 9,
        [Display(Name = "Surprise Count")]
        SurpriseCount = 10,
        [Display(Name = "Official Receipt")]
        OfficialReceipt = 11
    }
}
