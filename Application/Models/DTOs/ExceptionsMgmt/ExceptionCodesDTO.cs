using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.ExceptionsMgmt
{
    public enum DeviationStatusDTO
    {
        //[Display(Name = "Pending Approval")]
        //Pending = 1,
        Outstanding = 2,
        //Resolved = 3,
        [Display(Name = "Tag as Regularized")]
        Regularized = 4,
        [Display(Name = "Tag as Compliance")]
        ForCompliance = 5,
        //Deleted = 10
        [Display(Name = "Tag as Dispensed")]
        Dispensed = 7,
    }
    public enum ApprovalStatusDTO
    {
        [Display(Name = "Pending Approval")]
        PendingApproval = 0,
        Open = 1,
        Closed = 2,
    }
}
