using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Revisions
{
    public class RevisionsDTO
    {
        //[StringLength(30)]
        //[Display(Name = "Exception Ref. No.")]
        //public string ReferenceNo { get; set; }

        [StringLength(50)]
        public string Changes { get; set; }
        [StringLength(20)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        [StringLength(20)]
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDateTime { get; set; }
        [StringLength(25)]
        public string ActionTaken { get; set; }
        public bool IsProcessed { get; set; }
        [StringLength(300)]
        [Required(ErrorMessage = "Remarks field is required to proceed with the disapproval.")]
        public string ApprovalRemarks { get; set; }
    }
}
