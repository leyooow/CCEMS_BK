using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Monetary
{
    public enum NonMonetaryTypes
    {
        [Display(Name = "CIF Creation")]
        CIFCreation = 1,
        [Display(Name = "CIF Maintenance")]
        CIFMaintenance = 2,
        [Display(Name = "Account Opening")]
        AccountOpening = 3,
        [Display(Name = "Reactivated Dormant")]
        ReactivatedDormant = 4
    }
}
