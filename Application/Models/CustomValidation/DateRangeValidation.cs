using Application.Models.DTOs.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models.CustomValidation
{
    public class DateRangeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime currentDate = DateTime.Now;
            string errMsg = string.Empty;

            var vm = (GenerateMainReportsViewModel)validationContext.ObjectInstance;

            var dateFrom = vm.DateFrom;
            var dateTo = vm.DateTo;

            if (dateFrom > dateTo || dateTo < dateFrom)
            {
                errMsg += "Selected Date range is invalid. ";
            }

            if (errMsg != string.Empty)
            {
                return new ValidationResult(errMsg);
            }

            return ValidationResult.Success;
        }
    }
}
