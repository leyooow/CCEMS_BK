using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Report
{
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
