using Application.Models.DTOs.Revisions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.ExceptionsMgmt
{
    public class ExceptionCodeRevsDTO : RevisionsDTO
    {
        public int Id { get; set; }
        public string SubReferenceNo { get; set; }
        public int ExCode { get; set; }
        public ExceptionItemRevsDTO ExItem { get; set; }
        public string ExItemRefNo { get; set; }
        public DeviationStatusDTO DeviationStatus { get; set; }
        public DateTime DateCreated { get; set; }
        public ApprovalStatusDTO ApprovalStatus { get; set; }

        [NotMapped]
        public string ExCodeDescription { get; set; }

        public DateTime EntryDate { get; set; }

        public DateTime TaggingDate { get; set; }
    }
}
