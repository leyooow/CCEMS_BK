using Application.Models.DTOs.Report;
using Application.Models.Responses;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public class EPPlusPackages
    {
        private readonly string basePath = Settings.Config.REPORTS_DIRECTORY; //parameterize
        private Utilities _util;

        public EPPlusPackages()
        {
            _util = new Utilities();
        }
        public byte[] DailyOutstandingPackage(List<ReportContent> list, Report report, ExcelPackage package, List<string> footerContent, string userName, string approvedBy)
        {
            if (list.Count != 0)
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("DailyOutstanding");

                // Headers
                worksheet.Cells["A1"].Value = "DAILY EXCEPTION REPORT";
                worksheet.Cells["A2"].Value = string.Format("AREA: {0}", list.FirstOrDefault().Area);
                worksheet.Cells["A3"].Value = string.Format("COVERED DATE: {0}", report.CoverageDate.ToString("MM/dd/yyyy"));
                worksheet.Cells["A4"].Value = "Exception No.";
                worksheet.Cells["B4"].Value = "Branch Code";
                worksheet.Cells["C4"].Value = "DIVISION";
                worksheet.Cells["D4"].Value = "AREA";
                worksheet.Cells["E4"].Value = "Branch Name";
                worksheet.Cells["F4"].Value = "Date of Transaction";
                worksheet.Cells["G4"].Value = "AGING";
                worksheet.Cells["H4"].Value = "AGING CATEGORY";
                worksheet.Cells["I4"].Value = "Nature of Transaction/Process";
                worksheet.Cells["J4"].Value = "Account Number";
                worksheet.Cells["K4"].Value = "Account Name";
                worksheet.Cells["L4"].Value = "Deviation/Deficiency";
                worksheet.Cells["M4"].Value = "Risk Assessment";
                worksheet.Cells["N4"].Value = "Risk Classification";
                worksheet.Cells["O4"].Value = "Deviation Category";
                worksheet.Cells["P4"].Value = "Amount Involved";
                worksheet.Cells["Q4"].Value = "Employee/User Responsible";
                worksheet.Cells["R4"].Value = "Other Employee/s Responsible";
                worksheet.Cells["S4"].Value = "Remarks";
                worksheet.Cells["T4"].Value = "Branch Reply or Action Plan";
                worksheet.Cells["U4"].Value = "Encoded By";
                worksheet.Cells["V4"].Value = "DEVIATION APPROVER";
                worksheet.Cells["W4"].Value = "ROOT CAUSE";


                //Body
                var selectedList = list.Select(s => new
                {
                    s.ExceptionNo,
                    s.BranchCode,
                    s.Division,
                    s.Area,
                    s.BranchName,
                    s.TransactionDate,
                    s.Aging,
                    s.AgingCategory,
                    s.Process,
                    s.AccountNo,
                    s.AccountName,
                    s.Deviation,
                    s.Id,
                    s.RiskClassification,
                    s.DeviationCategory,
                    s.Amount,
                    s.PersonResponsible,
                    s.OtherPersonResponsible,
                    //s.Remarks,
                    s.ActionPlan,
                    s.EncodedBy,
                    s.DeviationApprover,
                    s.RootCause
                }).ToList();

                worksheet.Cells["A5"].LoadFromCollection(selectedList, false);

                int row = 5;
                foreach (var item in selectedList)
                {
                    worksheet.Cells["L" + row].Value = item.Deviation.Split(':').First();
                    worksheet.Cells["M" + row].Value = item.Deviation.Split(':').Last();
                    row++;
                }

                ApplyWorkSheetStyle(worksheet, "Daily", 22);
                // Footer
                worksheet.Cells[worksheet.Dimension.End.Row + 3, 1].Value = "Consolidated By:";
                worksheet.Cells[worksheet.Dimension.End.Row, 2].Value = userName;
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = "Reviewed and Approved By:";
                worksheet.Cells[worksheet.Dimension.End.Row, 7].Value = approvedBy;

                worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = footerContent[0];
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = footerContent.Count > 2 ? footerContent[2] : "{AOO Name Here}";

                worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = footerContent[1];
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = footerContent.Count > 2 ? footerContent[3] : "{Area Operations Officer}";
                ApplyWorkSheetStyle(worksheet, "Fonts", 0);

            }
            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] DailyRedFlagPackage(List<ReportContent> list, Report report, ExcelPackage package, List<string> footerContent, string userName, string approvedBy)
        {
            if (list.Count != 0)
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Red Flag");

                // Headers
                worksheet.Cells["A1"].Value = "RED FLAG REPORT";
                worksheet.Cells["A2"].Value = string.Format("AREA: {0}", list.FirstOrDefault().Area);
                worksheet.Cells["A3"].Value = string.Format("COVERED DATE: {0}", report.CoverageDate.ToString("MM/dd/yyyy"));
                worksheet.Cells["A4"].Value = "Exception No.";
                worksheet.Cells["B4"].Value = "Branch Code";
                worksheet.Cells["C4"].Value = "Branch Name";
                worksheet.Cells["D4"].Value = "AREA";
                worksheet.Cells["E4"].Value = "DIVISION";
                worksheet.Cells["F4"].Value = "Date of Transaction";
                worksheet.Cells["G4"].Value = "AGING";
                worksheet.Cells["H4"].Value = "AGING CATEGORY";
                worksheet.Cells["I4"].Value = "Nature of Transaction/Process";
                worksheet.Cells["J4"].Value = "Account Number";
                worksheet.Cells["K4"].Value = "Account Name";
                worksheet.Cells["L4"].Value = "Deviation/Deficiency";
                worksheet.Cells["M4"].Value = "Risk Assessment";
                worksheet.Cells["N4"].Value = "RISK CLASSIFICATION";
                worksheet.Cells["O4"].Value = "DEVIATION CATEGORY";
                worksheet.Cells["P4"].Value = "Amount Involved";
                worksheet.Cells["Q4"].Value = "Employee/User Responsible";
                worksheet.Cells["R4"].Value = "Other Employee/s Responsible";
                worksheet.Cells["S4"].Value = "Remarks";
                worksheet.Cells["T4"].Value = "Branch Reply or Action Plan";
                worksheet.Cells["U4"].Value = "Encoded By";
                worksheet.Cells["V4"].Value = "ROOT CAUSE";
                worksheet.Cells["W4"].Value = "DEVIATION APPROVER";

                //Body
                var selectedList = list.Select(s => new
                {
                    s.ExceptionNo,
                    s.BranchCode,
                    s.BranchName,
                    s.Area,
                    s.Division,
                    s.TransactionDate,
                    s.Aging,
                    s.AgingCategory,
                    s.Process,
                    s.AccountNo,
                    s.AccountName,
                    s.Deviation,
                    s.Id,
                    s.RiskClassification,
                    s.DeviationCategory,
                    s.Amount,
                    s.PersonResponsible,
                    s.OtherPersonResponsible,
                    s.Remarks,
                    s.ActionPlan,
                    s.EncodedBy,
                    s.RootCause,
                    s.DeviationApprover
                }).ToList();

                worksheet.Cells["A5"].LoadFromCollection(selectedList, false);

                int row = 5;
                foreach (var item in selectedList)
                {
                    worksheet.Cells["L" + row].Value = item.Deviation.Split(':').First();
                    worksheet.Cells["M" + row].Value = item.Deviation.Split(':').Last();
                    row++;
                }

                ApplyWorkSheetStyle(worksheet, "Daily", 22);

                // Footer
                worksheet.Cells[worksheet.Dimension.End.Row + 3, 1].Value = "Consolidated By:";
                worksheet.Cells[worksheet.Dimension.End.Row , 2].Value = userName;
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = "Reviewed and Approved By:";
                worksheet.Cells[worksheet.Dimension.End.Row, 7].Value = approvedBy;

                worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = footerContent[0];
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = footerContent.Count > 2 ? footerContent[2] : "{AOO Name Here}";

                worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = footerContent[1];
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = footerContent.Count > 2 ? footerContent[3] : "{Area Operations Officer}";
                ApplyWorkSheetStyle(worksheet, "Fonts", 0);

            }
            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] WeeklyNewAccountsPackage(List<ReportContent> list, Report report, ExcelPackage package, List<string> footerContent, string userName, string approvedBy)
        {
            if (list.Count != 0)
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("New Accounts");

                // Headers
                worksheet.Cells["A1"].Value = "WEEKLY NEW ACCOUNTS REPORT";
                worksheet.Cells["A2"].Value = string.Format("AREA: {0}", list.FirstOrDefault().Area);
                //worksheet.Cells["A3"].Value = string.Format("COVERED DATE: {0}", report.CoverageDate.ToString("MM/dd/yyyy"));
                worksheet.Cells["A3"].Value = string.Format("COVERED DATES: From {0} to {1}", list.FirstOrDefault().TransactionDate, list.LastOrDefault().TransactionDate);
                worksheet.Cells["A4"].Value = "Exception No.";
                worksheet.Cells["B4"].Value = "Branch Code";
                worksheet.Cells["C4"].Value = "DIVISION";
                worksheet.Cells["D4"].Value = "AREA";
                worksheet.Cells["E4"].Value = "Branch Name";
                worksheet.Cells["F4"].Value = "Date of Transaction";
                worksheet.Cells["G4"].Value = "AGING";
                worksheet.Cells["H4"].Value = "AGING CATEGORY";
                worksheet.Cells["I4"].Value = "Nature of Transaction/Process";
                worksheet.Cells["J4"].Value = "Account Number";
                worksheet.Cells["K4"].Value = "Account Name";
                worksheet.Cells["L4"].Value = "Deviation/Deficiency";
                worksheet.Cells["M4"].Value = "Risk Assessment";
                worksheet.Cells["N4"].Value = "DEVIATION CATEGORY";
                worksheet.Cells["O4"].Value = "RISK CLASSIFICATION";
                worksheet.Cells["P4"].Value = "Amount Involved";
                worksheet.Cells["Q4"].Value = "Employee/User Responsible";
                worksheet.Cells["R4"].Value = "Other Employee/s Responsible";
                worksheet.Cells["S4"].Value = "Remarks";
                worksheet.Cells["T4"].Value = "Branch Reply or Action Plan";
                worksheet.Cells["U4"].Value = "Encoded By";
                worksheet.Cells["V4"].Value = "DEVIATION APPROVER";
                worksheet.Cells["W4"].Value = "ROOT CAUSE";

                //Body
                var selectedList = list.Select(s => new
                {
                    s.ExceptionNo,
                    s.BranchCode,
                    s.Division,
                    s.Area,
                    s.BranchName,
                    s.TransactionDate,
                    s.Aging,
                    s.AgingCategory,
                    s.Process,
                    s.AccountNo,
                    s.AccountName,
                    s.Deviation,
                    s.Id,
                    s.DeviationCategory,
                    s.RiskClassification,
                    s.Amount,
                    s.PersonResponsible,
                    s.OtherPersonResponsible,
                    s.Remarks,
                    s.ActionPlan,
                    s.EncodedBy,
                    s.DeviationApprover,
                    s.RootCause
                }).ToList();

                worksheet.Cells["A5"].LoadFromCollection(selectedList, false);

                int row = 5;
                foreach (var item in selectedList)
                {
                    worksheet.Cells["L" + row].Value = item.Deviation.Split(':').First();
                    worksheet.Cells["M" + row].Value = item.Deviation.Split(':').Last();
                    row++;
                }

                ApplyWorkSheetStyle(worksheet, "Daily", 22);

                // Footer
                worksheet.Cells[worksheet.Dimension.End.Row + 3, 1].Value = "Consolidated By:";
                worksheet.Cells[worksheet.Dimension.End.Row , 2].Value = userName;
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = "Reviewed and Approved By:";
                worksheet.Cells[worksheet.Dimension.End.Row, 7].Value = approvedBy;

                worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = footerContent[0];
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = footerContent.Count > 2 ? footerContent[2] : "{AOO Name Here}";

                worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = footerContent[1];
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = footerContent.Count > 2 ? footerContent[3] : "{Area Operations Officer}";
                ApplyWorkSheetStyle(worksheet, "Fonts", 0);

            }
            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] WeeklyEscalationPackage(List<ReportContent> list, Report report, ExcelPackage package, List<string> footerContent, string userName, string approvedBy)
        {
            if (list.Count != 0)
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Escalation");

                // Headers
                worksheet.Cells["A1"].Value = "ESCALATION REPORT";
                worksheet.Cells["A2"].Value = string.Format("AREA: {0}", list.FirstOrDefault().Area);
                //worksheet.Cells["A3"].Value = string.Format("COVERED DATE: {0}", report.CoverageDate.ToString("MM/dd/yyyy"));
                worksheet.Cells["A3"].Value = string.Format("COVERED DATES: From {0} to {1}", list.FirstOrDefault().TransactionDate, list.LastOrDefault().TransactionDate);
                worksheet.Cells["A4"].Value = "Exception No.";
                worksheet.Cells["B4"].Value = "Branch Code";
                worksheet.Cells["C4"].Value = "DIVISION";
                worksheet.Cells["D4"].Value = "AREA";
                worksheet.Cells["E4"].Value = "Branch Name";
                worksheet.Cells["F4"].Value = "Date of Transaction";
                worksheet.Cells["G4"].Value = "AGING";
                worksheet.Cells["H4"].Value = "AGING CATEGORY";
                worksheet.Cells["I4"].Value = "Nature of Transaction/Process";
                worksheet.Cells["J4"].Value = "Account Number";
                worksheet.Cells["K4"].Value = "Account Name";
                worksheet.Cells["L4"].Value = "Deviation/Deficiency";
                worksheet.Cells["M4"].Value = "Deviation Category";
                worksheet.Cells["N4"].Value = "Risk Classification";
                worksheet.Cells["O4"].Value = "Risk Assessment";
                worksheet.Cells["P4"].Value = "Amount Involved";
                worksheet.Cells["Q4"].Value = "Employee/User Responsible";
                worksheet.Cells["R4"].Value = "Other Employee/s Responsible";
                worksheet.Cells["S4"].Value = "Remarks";
                worksheet.Cells["T4"].Value = "Branch Reply or Action Plan";
                worksheet.Cells["U4"].Value = "Encoded By";
                worksheet.Cells["V4"].Value = "DEVIATION APPROVER";
                worksheet.Cells["W4"].Value = "ROOT CAUSE";

                //Body
                var selectedList = list.Select(s => new
                {
                    s.ExceptionNo,
                    s.BranchCode,
                    s.Division,
                    s.Area,
                    s.BranchName,
                    s.TransactionDate,
                    s.Aging,
                    s.AgingCategory,
                    s.Process,
                    s.AccountNo,
                    s.AccountName,
                    s.Deviation,
                    s.DeviationCategory,
                    s.RiskClassification,
                    s.Id,
                    s.Amount,
                    s.PersonResponsible,
                    s.OtherPersonResponsible,
                    s.Remarks,
                    s.ActionPlan,
                    s.EncodedBy,
                    s.DeviationApprover,
                    s.RootCause
                }).ToList();

                worksheet.Cells["A5"].LoadFromCollection(selectedList, false);

                int row = 5;
                foreach (var item in selectedList)
                {
                    worksheet.Cells["N" + row].Value = item.Deviation.Split(':').First();
                    worksheet.Cells["O" + row].Value = item.Deviation.Split(':').Last();
                    row++;
                }

                ApplyWorkSheetStyle(worksheet, "Daily", 22);

                // Footer
                worksheet.Cells[worksheet.Dimension.End.Row + 3, 1].Value = "Consolidated By:";
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = "Reviewed and Approved By:";

                worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = footerContent[0];
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = footerContent.Count > 2 ? footerContent[2] : ""; // AOO name here

                worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = footerContent[1];
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = footerContent.Count > 2 ? footerContent[3] : ""; // Approver's position
                ApplyWorkSheetStyle(worksheet, "Fonts", 0);

            }
            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] MonthlyOutstandingPackage(List<ReportContent> list, Report report, ExcelPackage package, List<string> footerContent, string userName, string approvedBy)
        {
            if (list.Count != 0)
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Monthly Outstanding");

                // Headers
                if ((ReportCategory)report.ReportCategory == ReportCategory.AllOutstanding1)
                {
                    worksheet.Cells["A1"].Value = "ALL OUTSTANDING EXCEPTIONS REPORT(MONETARY & MISC)";
                }
                else if ((ReportCategory)report.ReportCategory == ReportCategory.AllOutstanding2)
                {
                    worksheet.Cells["A1"].Value = "ALL OUTSTANDING EXCEPTIONS REPORT(Non-Monetary)";
                }
                string getDate = list.OrderBy(x => x.TransactionDate).FirstOrDefault().TransactionDate.Replace("/","-") + " 12:00:00.0000000";
                var date = Convert.ToDateTime(Convert.ToDateTime(getDate).ToString("yyyy-MM-dd"));
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                worksheet.Cells["A2"].Value = string.Format("FOR THE MONTH OF {0} {1}", firstDayOfMonth.ToString("MM/dd/yyyy"), lastDayOfMonth.ToString("MM/dd/yyyy"));
                worksheet.Cells["A3"].Value = string.Format("AREA: {0}", list.FirstOrDefault().Area);
                //worksheet.Cells["A4"].Value = string.Format("REPORT DATE {0}", DateTime.Now.ToString("MM-dd-yyyy"));
                // worksheet.Cells["A2"].Value = string.Format("AREA: {0}", list.FirstOrDefault().Area);
                worksheet.Cells["A4"].Value = "Exception No.";
                worksheet.Cells["B4"].Value = "Branch Code";
                worksheet.Cells["C4"].Value = "DIVISION";
                worksheet.Cells["D4"].Value = "AREA";
                worksheet.Cells["E4"].Value = "Branch Name";
                worksheet.Cells["F4"].Value = "Date of Transaction";
                worksheet.Cells["G4"].Value = "AGING";
                worksheet.Cells["H4"].Value = "AGING CATEGORY";
                worksheet.Cells["I4"].Value = "Nature of Transaction/Process";
                worksheet.Cells["J4"].Value = "Account Number";
                worksheet.Cells["K4"].Value = "Account Name";
                worksheet.Cells["L4"].Value = "Deviation/Deficiency";
                worksheet.Cells["M4"].Value = "Risk Assessment";
                worksheet.Cells["N4"].Value = "Deviation Category";
                worksheet.Cells["O4"].Value = "Risk Classification";
                worksheet.Cells["P4"].Value = "Amount Involved";
                worksheet.Cells["Q4"].Value = "Employee/User Responsible";
                worksheet.Cells["R4"].Value = "Other Employee/s Responsible";
                worksheet.Cells["S4"].Value = "Remarks";
                worksheet.Cells["T4"].Value = "Branch Reply or Action Plan";
                worksheet.Cells["U4"].Value = "Encoded By";
                worksheet.Cells["V4"].Value = "DEVIATION APPROVER";
                worksheet.Cells["W4"].Value = "ROOT CAUSE";

                //Body
                var selectedList = list.Select(s => new
                {
                    s.ExceptionNo,
                    s.BranchCode,
                    s.Division,
                    s.Area,
                    s.BranchName,
                    s.TransactionDate,
                    s.Aging,
                    s.AgingCategory,
                    s.Process,
                    s.AccountNo,
                    s.AccountName,
                    s.Deviation,
                    s.Id,
                    s.DeviationCategory,
                    s.RiskClassification,
                    s.Amount,
                    s.PersonResponsible,
                    s.OtherPersonResponsible,
                    s.Remarks,
                    s.ActionPlan,
                    s.EncodedBy,
                    s.DeviationApprover,
                    s.RootCause
                }).ToList();

                worksheet.Cells["A5"].LoadFromCollection(selectedList, false);

                int row = 5;
                foreach (var item in selectedList)
                {
                    worksheet.Cells["L" + row].Value = item.Deviation.Split(':').First();
                    worksheet.Cells["M" + row].Value = item.Deviation.Split(':').Last();
                    row++;
                }

                ApplyWorkSheetStyle(worksheet, "Daily", 22);

                // Footer
                worksheet.Cells[worksheet.Dimension.End.Row + 3, 1].Value = "Consolidated By:";
                //worksheet.Cells[worksheet.Dimension.End.Row , 2].Value = userName;
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = "Reviewed and Approved By:";
                //worksheet.Cells[worksheet.Dimension.End.Row, 7].Value = approvedBy;

                worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = footerContent[0];
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = footerContent.Count > 2 ? footerContent[2] : "{AOO Name Here}";

                worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = footerContent[1];
                worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = footerContent.Count > 2 ? footerContent[3] : "{Area Operations Officer}";
                ApplyWorkSheetStyle(worksheet, "Fonts", 0);

            }
            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] GenerateRegularizationTATReport(List<RegularizationTATFormat> list, ExcelPackage package, DateTime dateFrom, DateTime dateTo)
        {

            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("TAT");
            // Headers
            worksheet.Cells["A1"].Value = "REGULARIZATION TAT REPORT";
            worksheet.Cells["A2"].Value = string.Format("COVERED DATES: From {0} to {1}", dateFrom.ToString("MM/dd/yyyy"), dateTo.ToString("MM/dd/yyyy"));
            worksheet.Cells["A3"].Value = "Employee ID";
            worksheet.Cells["B3"].Value = "Employee/s Responsible";
            worksheet.Cells["C3"].Value = "Branch Code";
            worksheet.Cells["D3"].Value = "Branch Name";
            worksheet.Cells["E3"].Value = "Exception Number";
            worksheet.Cells["F3"].Value = "Sub-Exception Number";
            worksheet.Cells["G3"].Value = "Deviation/Deficiency";
            worksheet.Cells["H3"].Value = "Deviation Category";
            worksheet.Cells["I3"].Value = "Transaction Date";
            worksheet.Cells["J3"].Value = "Date Regularized";
            worksheet.Cells["K3"].Value = "# of Days Outstanding";
            worksheet.Cells["L3"].Value = "Status";

            worksheet.Cells["A4"].LoadFromCollection(list, false);

            // Footer
            // Compute Average
            double average = 0;
            double totalAmount = 0;
            foreach (var item in list)
            {
                totalAmount += item.DaysOutstanding;
            }
            average = totalAmount / list.Count;

            //Get Equivalent Rating
            int ratings = GetRTATRatings(average);

            worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = "Average # of Days O/S";
            var averageEndRow = worksheet.Dimension.End.Row - 1;
            worksheet.Cells[worksheet.Dimension.End.Row, 11].Formula = $"=AVERAGE(K4:K{averageEndRow})";
            worksheet.Cells[worksheet.Dimension.End.Row, 1, worksheet.Dimension.End.Row, 10].Merge = true;

            worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = "EQUIVALENT RATING";
            worksheet.Cells[worksheet.Dimension.End.Row, 11].Value = ratings;
            worksheet.Cells[worksheet.Dimension.End.Row, 1, worksheet.Dimension.End.Row, 10].Merge = true;

            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] GeneratePervasivenessReport(List<PervasivenessFormat> list, ExcelPackage package, int ave, DateTime dateFrom, DateTime dateTo)
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Pervasiveness");
            // Headers
            worksheet.Cells["A1"].Value = "PERVASIVENESS REPORT";
            worksheet.Cells["A2"].Value = string.Format("COVERED DATES: From {0} to {1}", dateFrom.ToString("MM/dd/yyyy"), dateTo.ToString("MM/dd/yyyy"));
            worksheet.Cells["A3"].Value = "Employee ID";
            worksheet.Cells["B3"].Value = "Employee Name";
            worksheet.Cells["C3"].Value = "Branch Code";
            worksheet.Cells["D3"].Value = "Branch Name";
            worksheet.Cells["E3"].Value = "Exception Number";
            worksheet.Cells["F3"].Value = "Sub-Exception Number";
            worksheet.Cells["G3"].Value = "Deviation/Deficiency";
            worksheet.Cells["H3"].Value = "Deviation Category";
            worksheet.Cells["I3"].Value = "# of Times Comitted";
            worksheet.Cells["J3"].Value = "Date Recorded";
            worksheet.Cells["K3"].Value = "Status";

            worksheet.Cells["A4"].LoadFromCollection(list, false);

            // Footer
            // Get Equivalent Rating
            int ratings = GetPervasiveRatings(ave, list.Any());

            worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = "TOTAL NUMBER OF RECURRING EXCEPTIONS";
            var averageEndRow = worksheet.Dimension.End.Row - 1;
            worksheet.Cells[worksheet.Dimension.End.Row, 9].Value = ave;
            worksheet.Cells[worksheet.Dimension.End.Row, 1, worksheet.Dimension.End.Row, 8].Merge = true;

            worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = "EQUIVALENT RATING";
            worksheet.Cells[worksheet.Dimension.End.Row, 9].Value = ratings;
            worksheet.Cells[worksheet.Dimension.End.Row, 1, worksheet.Dimension.End.Row, 8].Merge = true;

            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] GenerateExceptionAdhocs(List<AllOutstandingMonthly> list, ExcelPackage package, AdhocStatus status, DateTime dateFrom, DateTime dateTo)
        {
            ExcelWorksheet worksheet = null;

            if (status == AdhocStatus.Regularized)
            {
                worksheet = package.Workbook.Worksheets.Add("Regularized");
                worksheet.Cells["A1"].Value = "ALL REGULARIZED EXCEPTIONS REPORT";

                // Regularized Exception Headers                       
                worksheet.Cells["A3"].Value = string.Format("AREA: {0}", list.FirstOrDefault().Area);
                worksheet.Cells["A4"].Value = string.Format("COVERED DATES: From {0} to {1}", dateFrom.ToString("MM/dd/yyyy"), dateTo.ToString("MM/dd/yyyy"));
                worksheet.Cells["A5"].Value = "Exception No.";
                worksheet.Cells["C5"].Value = "Branch Name";
                worksheet.Cells["D5"].Value = "AREA";
                worksheet.Cells["E5"].Value = "DIVISION";
                worksheet.Cells["B5"].Value = "Branch Code";
                worksheet.Cells["F5"].Value = "Date of Transaction";
                worksheet.Cells["G5"].Value = "Regularization Date";
                worksheet.Cells["H5"].Value = "AGING";
                worksheet.Cells["I5"].Value = "AGING CATEGORY";
                worksheet.Cells["J5"].Value = "Nature of Transaction/Process";
                worksheet.Cells["K5"].Value = "Account Number";
                worksheet.Cells["L5"].Value = "Account Name";
                worksheet.Cells["M5"].Value = "Deviation/Deficiency";
                worksheet.Cells["N5"].Value = "RISK CLASSIFICATION";
                worksheet.Cells["O5"].Value = "DEVIATION CATEGORY";
                worksheet.Cells["P5"].Value = "Amount Involved";
                worksheet.Cells["Q5"].Value = "Employee/User Responsible";
                worksheet.Cells["R5"].Value = "Other Employee/s Responsible";
                worksheet.Cells["S5"].Value = "Remarks";
                worksheet.Cells["T5"].Value = "Branch Reply or Action Plan";
                worksheet.Cells["U5"].Value = "Encoded By";
                worksheet.Cells["V5"].Value = "ROOT CAUSE";
                worksheet.Cells["W5"].Value = "DEVIATION APPROVER";

                //Body
                worksheet.Cells["A6"].LoadFromCollection(list, false);
                ApplyWorkSheetStyle(worksheet, "Monthly", 23);
            }
            else if (status == AdhocStatus.ForCompliance)
            {
                worksheet = package.Workbook.Worksheets.Add("For Compliance");
                worksheet.Cells["A1"].Value = "EXCEPTIONS TAGGED AS FOR COMPLIANCE REPORT";

                // For Compliance Exception Headers                       
                worksheet.Cells["A3"].Value = string.Format("AREA: {0}", "");
                worksheet.Cells["A4"].Value = string.Format("COVERED DATES: From {0} to {1}", "", "");
                worksheet.Cells["A5"].Value = "Exception No.";
                worksheet.Cells["C5"].Value = "Branch Name";
                worksheet.Cells["D5"].Value = "AREA";
                worksheet.Cells["E5"].Value = "DIVISION";
                worksheet.Cells["B5"].Value = "Branch Code";
                worksheet.Cells["F5"].Value = "Date of Transaction";
                worksheet.Cells["G5"].Value = "Date Tagged for Compliance";
                worksheet.Cells["H5"].Value = "AGING";
                worksheet.Cells["I5"].Value = "AGING CATEGORY";
                worksheet.Cells["J5"].Value = "Nature of Transaction/Process";
                worksheet.Cells["K5"].Value = "Account Number";
                worksheet.Cells["L5"].Value = "Account Name";
                worksheet.Cells["M5"].Value = "Deviation/Deficiency";
                worksheet.Cells["N5"].Value = "RISK CLASSIFICATION";
                worksheet.Cells["O5"].Value = "DEVIATION CATEGORY";
                worksheet.Cells["P5"].Value = "Amount Involved";
                worksheet.Cells["Q5"].Value = "Employee/User Responsible";
                worksheet.Cells["R5"].Value = "Other Employee/s Responsible";
                worksheet.Cells["S5"].Value = "Remarks";
                worksheet.Cells["T5"].Value = "Branch Reply or Action Plan";
                worksheet.Cells["U5"].Value = "Encoded By";
                worksheet.Cells["V5"].Value = "ROOT CAUSE";
                worksheet.Cells["W5"].Value = "DEVIATION APPROVER";

                //Body
                worksheet.Cells["A6"].LoadFromCollection(list, false);
                ApplyWorkSheetStyle(worksheet, "Monthly", 23);
            }
            else
            {
                worksheet = package.Workbook.Worksheets.Add("Deleted");
                worksheet.Cells["A1"].Value = "ALL DELETED EXCEPTIONS REPORT";

                //Deleted Exception Headers                       
                worksheet.Cells["A3"].Value = string.Format("AREA: {0}", "");
                worksheet.Cells["A4"].Value = string.Format("COVERED DATES: From {0} to {1}", "", "");
                worksheet.Cells["A5"].Value = "Exception No.";
                worksheet.Cells["C5"].Value = "Branch Name";
                worksheet.Cells["D5"].Value = "AREA";
                worksheet.Cells["E5"].Value = "DIVISION";
                worksheet.Cells["B5"].Value = "Branch Code";
                worksheet.Cells["F5"].Value = "Date of Transaction";
                worksheet.Cells["G5"].Value = "Deletion Date";
                worksheet.Cells["H5"].Value = "AGING";
                worksheet.Cells["I5"].Value = "AGING CATEGORY";
                worksheet.Cells["J5"].Value = "Nature of Transaction/Process";
                worksheet.Cells["K5"].Value = "Account Number";
                worksheet.Cells["L5"].Value = "Account Name";
                worksheet.Cells["M5"].Value = "Deviation/Deficiency";
                worksheet.Cells["N5"].Value = "RISK CLASSIFICATION";
                worksheet.Cells["O5"].Value = "DEVIATION CATEGORY";
                worksheet.Cells["P5"].Value = "Amount Involved";
                worksheet.Cells["Q5"].Value = "Employee/User Responsible";
                worksheet.Cells["R5"].Value = "Other Employee/s Responsible";
                worksheet.Cells["S5"].Value = "Remarks";
                worksheet.Cells["T5"].Value = "Reason for Deletion";
                worksheet.Cells["U5"].Value = "Branch Reply or Action Plan";
                worksheet.Cells["V5"].Value = "Encoded By";
                worksheet.Cells["W5"].Value = "ROOT CAUSE";
                worksheet.Cells["X5"].Value = "DEVIATION APPROVER";

                //Body
                worksheet.Cells["A6"].LoadFromCollection(list, false);
                ApplyWorkSheetStyle(worksheet, "Monthly", 24);
            }

            // Footer
            //worksheet.Cells[worksheet.Dimension.End.Row + 3, 1].Value = "Consolidated By:";
            //worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = "Reviewed and Approved By:";

            //worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = "{BOCC Name Here}";
            //worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = "{AOO Name Here}";

            //worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].Value = "{BOCC Specific Position}";
            //worksheet.Cells[worksheet.Dimension.End.Row, 6].Value = "{Area Operations Officer}";
            //ApplyWorkSheetStyle(worksheet, "Fonts", 0);

            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] GenerateAllDeletedAdhoc(List<AdhocsAllDeleted> list, ExcelPackage package, AdhocStatus status, DateTime dateFrom, DateTime dateTo)
        {
            ExcelWorksheet worksheet = null;


            worksheet = package.Workbook.Worksheets.Add("Deleted");
            worksheet.Cells["A1"].Value = "ALL DELETED EXCEPTIONS REPORT";

            //Deleted Exception Headers                       
            worksheet.Cells["A3"].Value = string.Format("AREA: {0}", list.FirstOrDefault().Area);
            worksheet.Cells["A4"].Value = string.Format("COVERED DATES: From {0} to {1}", dateFrom.ToString("MM/dd/yyyy"), dateTo.ToString("MM/dd/yyyy"));
            worksheet.Cells["A5"].Value = "Exception No.";
            worksheet.Cells["C5"].Value = "Branch Name";
            worksheet.Cells["D5"].Value = "AREA";
            worksheet.Cells["E5"].Value = "DIVISION";
            worksheet.Cells["B5"].Value = "Branch Code";
            worksheet.Cells["F5"].Value = "Date of Transaction";
            worksheet.Cells["G5"].Value = "Deletion Date";
            worksheet.Cells["H5"].Value = "AGING";
            worksheet.Cells["I5"].Value = "AGING CATEGORY";
            worksheet.Cells["J5"].Value = "Nature of Transaction/Process";
            worksheet.Cells["K5"].Value = "Account Number";
            worksheet.Cells["L5"].Value = "Account Name";
            worksheet.Cells["M5"].Value = "Deviation/Deficiency";
            worksheet.Cells["N5"].Value = "RISK CLASSIFICATION";
            worksheet.Cells["O5"].Value = "DEVIATION CATEGORY";
            worksheet.Cells["P5"].Value = "Amount Involved";
            worksheet.Cells["Q5"].Value = "Employee/User Responsible";
            worksheet.Cells["R5"].Value = "Other Employee/s Responsible";
            worksheet.Cells["S5"].Value = "Remarks";
            worksheet.Cells["T5"].Value = "Reason for Deletion";
            worksheet.Cells["U5"].Value = "Branch Reply or Action Plan";
            worksheet.Cells["V5"].Value = "Encoded By";
            worksheet.Cells["W5"].Value = "ROOT CAUSE";
            worksheet.Cells["X5"].Value = "DEVIATION APPROVER";

            //Body
            worksheet.Cells["A6"].LoadFromCollection(list, false);
            ApplyWorkSheetStyle(worksheet, "Monthly", 24);

            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] GenerateRegularizeAdhoc(List<AdhocsOthers> list, ExcelPackage package, AdhocStatus status, DateTime dateFrom, DateTime dateTo)
        {
            ExcelWorksheet worksheet = null;

            if (status == AdhocStatus.Regularized)
            {
                worksheet = package.Workbook.Worksheets.Add("Regularized");
                worksheet.Cells["A1"].Value = "ALL REGULARIZED EXCEPTIONS REPORT";

                // Regularized Exception Headers                       
                worksheet.Cells["A3"].Value = string.Format("AREA: {0}", list.FirstOrDefault().Area);
                worksheet.Cells["A4"].Value = string.Format("COVERED DATES: From {0} to {1}", dateFrom.ToString("MM/dd/yyyy"), dateTo.ToString("MM/dd/yyyy"));
                worksheet.Cells["A5"].Value = "Exception No.";
                worksheet.Cells["C5"].Value = "Branch Name";
                worksheet.Cells["D5"].Value = "AREA";
                worksheet.Cells["E5"].Value = "DIVISION";
                worksheet.Cells["B5"].Value = "Branch Code";
                worksheet.Cells["F5"].Value = "Date of Transaction";
                worksheet.Cells["G5"].Value = "Regularization Date";
                worksheet.Cells["H5"].Value = "AGING";
                worksheet.Cells["I5"].Value = "AGING CATEGORY";
                worksheet.Cells["J5"].Value = "Nature of Transaction/Process";
                worksheet.Cells["K5"].Value = "Account Number";
                worksheet.Cells["L5"].Value = "Account Name";
                worksheet.Cells["M5"].Value = "Deviation/Deficiency";
                worksheet.Cells["N5"].Value = "RISK CLASSIFICATION";
                worksheet.Cells["O5"].Value = "DEVIATION CATEGORY";
                worksheet.Cells["P5"].Value = "Amount Involved";
                worksheet.Cells["Q5"].Value = "Employee/User Responsible";
                worksheet.Cells["R5"].Value = "Other Employee/s Responsible";
                worksheet.Cells["S5"].Value = "Remarks";
                worksheet.Cells["T5"].Value = "Branch Reply or Action Plan";
                worksheet.Cells["U5"].Value = "Encoded By";
                worksheet.Cells["V5"].Value = "ROOT CAUSE";
                worksheet.Cells["W5"].Value = "DEVIATION APPROVER";

                //Body
                worksheet.Cells["A6"].LoadFromCollection(list, false);
                ApplyWorkSheetStyle(worksheet, "Monthly", 23);
            }

            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] GenerateForComplianceAdhoc(List<AdhocsOthers> list, ExcelPackage package, AdhocStatus status, DateTime dateFrom, DateTime dateTo)
        {
            ExcelWorksheet worksheet = null;

            if (status == AdhocStatus.ForCompliance)
            {
                worksheet = package.Workbook.Worksheets.Add("For Compliance");
                worksheet.Cells["A1"].Value = "EXCEPTIONS TAGGED AS FOR COMPLIANCE REPORT";

                // For Compliance Exception Headers                       
                worksheet.Cells["A3"].Value = string.Format("AREA: {0}", "");
                worksheet.Cells["A4"].Value = string.Format("COVERED DATES: From {0} to {1}", dateFrom.ToString("MM/dd/yyyy"), dateTo.ToString("MM/dd/yyyy"));
                worksheet.Cells["A5"].Value = "Exception No.";
                worksheet.Cells["C5"].Value = "Branch Name";
                worksheet.Cells["D5"].Value = "AREA";
                worksheet.Cells["E5"].Value = "DIVISION";
                worksheet.Cells["B5"].Value = "Branch Code";
                worksheet.Cells["F5"].Value = "Date of Transaction";
                worksheet.Cells["G5"].Value = "Date Tagged for Compliance";
                worksheet.Cells["H5"].Value = "AGING";
                worksheet.Cells["I5"].Value = "AGING CATEGORY";
                worksheet.Cells["J5"].Value = "Nature of Transaction/Process";
                worksheet.Cells["K5"].Value = "Account Number";
                worksheet.Cells["L5"].Value = "Account Name";
                worksheet.Cells["M5"].Value = "Deviation/Deficiency";
                worksheet.Cells["N5"].Value = "RISK CLASSIFICATION";
                worksheet.Cells["O5"].Value = "DEVIATION CATEGORY";
                worksheet.Cells["P5"].Value = "Amount Involved";
                worksheet.Cells["Q5"].Value = "Employee/User Responsible";
                worksheet.Cells["R5"].Value = "Other Employee/s Responsible";
                worksheet.Cells["S5"].Value = "Remarks";
                worksheet.Cells["T5"].Value = "Branch Reply or Action Plan";
                worksheet.Cells["U5"].Value = "Encoded By";
                worksheet.Cells["V5"].Value = "ROOT CAUSE";
                worksheet.Cells["W5"].Value = "DEVIATION APPROVER";

                //Body
                worksheet.Cells["A6"].LoadFromCollection(list, false);
                ApplyWorkSheetStyle(worksheet, "Monthly", 23);
            }

            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] GenerateForDispensedAdhoc(List<AdhocsOthers> list, ExcelPackage package, AdhocStatus status, DateTime dateFrom, DateTime dateTo)
        {
            ExcelWorksheet worksheet = null;

            if (status == AdhocStatus.Dispensed)
            {
                worksheet = package.Workbook.Worksheets.Add("For Dispensed");
                worksheet.Cells["A1"].Value = "EXCEPTIONS TAGGED AS FOR DISPENSED REPORT";

                // For Compliance Exception Headers                       
                worksheet.Cells["A3"].Value = string.Format("AREA: {0}", "");
                worksheet.Cells["A4"].Value = string.Format("COVERED DATES: From {0} to {1}", dateFrom.ToString("MM/dd/yyyy"), dateTo.ToString("MM/dd/yyyy"));
                worksheet.Cells["A5"].Value = "Exception No.";
                worksheet.Cells["C5"].Value = "Branch Name";
                worksheet.Cells["D5"].Value = "AREA";
                worksheet.Cells["E5"].Value = "DIVISION";
                worksheet.Cells["B5"].Value = "Branch Code";
                worksheet.Cells["F5"].Value = "Date of Transaction";
                worksheet.Cells["G5"].Value = "Date Tagged for Compliance";
                worksheet.Cells["H5"].Value = "AGING";
                worksheet.Cells["I5"].Value = "AGING CATEGORY";
                worksheet.Cells["J5"].Value = "Nature of Transaction/Process";
                worksheet.Cells["K5"].Value = "Account Number";
                worksheet.Cells["L5"].Value = "Account Name";
                worksheet.Cells["M5"].Value = "Deviation/Deficiency";
                worksheet.Cells["N5"].Value = "RISK CLASSIFICATION";
                worksheet.Cells["O5"].Value = "DEVIATION CATEGORY";
                worksheet.Cells["P5"].Value = "Amount Involved";
                worksheet.Cells["Q5"].Value = "Employee/User Responsible";
                worksheet.Cells["R5"].Value = "Other Employee/s Responsible";
                worksheet.Cells["S5"].Value = "Remarks";
                worksheet.Cells["T5"].Value = "Branch Reply or Action Plan";
                worksheet.Cells["U5"].Value = "Encoded By";
                worksheet.Cells["V5"].Value = "ROOT CAUSE";
                worksheet.Cells["W5"].Value = "DEVIATION APPROVER";

                //Body
                worksheet.Cells["A6"].LoadFromCollection(list, false);
                ApplyWorkSheetStyle(worksheet, "Monthly", 23);
            }

            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }
        public byte[] GenerateRegularReport(List<ReportContent> list, ExcelPackage package)
        {


            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        public byte[] PulloutRequestPackage(List<ReportContent> list, Report report, ExcelPackage package, 
                                            List<string> footerContent, string userName, string approvedBy, string seqNo)
        {
            if (list.Count != 0)
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Pullout"];

                //worksheet.Cells["B15"].Value = "Documents sent out to: " + list.Take(1).SingleOrDefault().BranchName;
                worksheet.Cells["C2"].Value = list.Take(1).SingleOrDefault().BranchName + "-" + _util.PaddingFieldValue(list.Take(1).SingleOrDefault().BranchCode, false, 3, "0");
                worksheet.Cells["C3"].Value = DateTime.Now.ToString("MM/dd/yyyy"); 
                worksheet.Cells["C4"].Value = list.Take(1).SingleOrDefault().BranchName + "-" + _util.PaddingFieldValue(list.Take(1).SingleOrDefault().BranchCode, false, 3, "0");
                worksheet.Cells["G2"].Value = DateTime.Now.ToString("yyyy") + "-" + _util.PaddingFieldValue(list.Take(1).SingleOrDefault().BranchCode, false, 3, "0") + "-" + seqNo;
 
                int rowCounter = 7;
                foreach (var item in list)
                {

                    worksheet.Cells["A" + rowCounter].Value = item.TransactionDate;
                    worksheet.Cells["B" + rowCounter].Value = item.Process;
                    worksheet.Cells["C" + rowCounter].Value = item.AccountNo;
                    worksheet.Cells["D" + rowCounter].Value = item.AccountName;
                    worksheet.Cells["E" + rowCounter].Value = item.Amount;

                    rowCounter++;
                    worksheet.InsertRow(rowCounter, 1);

                }
                worksheet.Cells["B" + (rowCounter + 2)].Value = "Documents sent out to: " + list.Take(1).SingleOrDefault().BranchName;

                //ApplyWorkSheetStyle(worksheet, "Pullout", 13);

            }

            byte[] excelData = package.GetAsByteArray();
            return excelData;
        }

        private void ApplyWorkSheetStyle(ExcelWorksheet ws, string classification, int rowCount)
        {
            if (classification == "Daily")
            {
                using (ExcelRange header = ws.Cells[4, 1, 4, rowCount])
                {
                    header.Style.Font.Bold = true;
                    header.Style.ShrinkToFit = false;
                    header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    header.Style.Fill.BackgroundColor.SetColor(Color.White);
                    //tbheader.Style.Font.Color.SetColor(Color.White);
                }

                using (ExcelRange workSheet = ws.Cells[4, 1, ws.Dimension.End.Row, ws.Dimension.End.Column])
                {
                    workSheet.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                }
            }


            if (classification == "Monthly")
            {
                using (ExcelRange header = ws.Cells[5, 1, 5, rowCount])
                {
                    header.Style.Font.Bold = true;
                    header.Style.ShrinkToFit = false;
                    header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    header.Style.Fill.BackgroundColor.SetColor(Color.White);
                    //tbheader.Style.Font.Color.SetColor(Color.White);
                }

                using (ExcelRange workSheet = ws.Cells[5, 1, ws.Dimension.End.Row, ws.Dimension.End.Column])
                {
                    workSheet.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                }
            }

            if (classification == "Fonts")
            {
                using (ExcelRange workSheet = ws.Cells[4, 1, ws.Dimension.End.Row, ws.Dimension.End.Column])
                {
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    workSheet.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Style.Font.Size = 10;
                    workSheet.Style.WrapText = true;
                }
            }

            if (classification == "Pullout")
            {
                int startRow = 7; // Start row
                int maxColumn = 13; // Maximum column
                int currentRow = startRow; // Current row
                bool continueStyling = true; // Flag to continue applying style

                // Iterate through rows until an empty cell is encountered or no value
                while (continueStyling)
                {
                    // Get the range of cells in the current row
                    var currentRowCells = ws.Cells[currentRow, 1, currentRow, maxColumn];

                    // Iterate through cells in the current row
                    foreach (var cell in currentRowCells)
                    {
                        // Apply border to the cell
                        if (cell.Start.Column <= maxColumn)
                        {
                            // Apply border to top, right, bottom, and left of the cell
                            cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        }


                    }

                    // Increment the current row for the next iteration
                    currentRow++;

                    // Check if the cell in the next row is empty or has no value
                    var nextRowCell = ws.Cells[currentRow, 1];
                    if (string.IsNullOrEmpty(nextRowCell.Text))
                    {
                        // If empty cell is encountered, stop applying style to subsequent rows
                        continueStyling = false;
                    }
                }


                //using (ExcelRange header = ws.Cells[7, 1, 8, 13])
                //{
                //    header.Style.Font.Bold = true;
                //    header.Style.ShrinkToFit = false;
                //    header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //    header.Style.Fill.BackgroundColor.SetColor(Color.White);
                //    //tbheader.Style.Font.Color.SetColor(Color.White);
                //}

                //using (ExcelRange header = ws.Cells[5, 1, 6, 13])
                //{
                //    header.Style.Font.Bold = true;
                //    header.Style.ShrinkToFit = false;
                //    header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //    header.Style.Fill.BackgroundColor.SetColor(Color.White);
                //    //tbheader.Style.Font.Color.SetColor(Color.White);
                //}

                //using (ExcelRange workSheet = ws.Cells[5, 1, ws.Dimension.End.Row, ws.Dimension.End.Column])
                //{
                //    workSheet.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //    workSheet.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                //    workSheet.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //    workSheet.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                //}
            }


        }

        private int GetRTATRatings(double average)
        {
            int ratings = 0;

            if (average == 0)
            {
                ratings = 5;
            }
            else if (average < 5)
            {
                ratings = 4;
            }
            else if (average >= 5 && average <= 7)
            {
                ratings = 3;
            }
            else if (average >= 8 && average <= 29)
            {
                ratings = 2;
            }
            else if (average >= 30)
            {
                ratings = 1;
            }
            return ratings;
        }

        private int GetPervasiveRatings(double average, bool isNoException)
        {

            int ratings = 0;
            //TODO: Parameterize and Revised Pervasiveness Ratings per Ma'am Alyn via email. (Excluded on BRD)
            //Subject: PERVASIVENESS KRA FOR CCEMS, DateTime: 09/20/2021 01:43 PM
            //Hardcoded for now..
            //string[] pr_params = Settings.Config.PR_PARAMS.Split(',');

            #region Initial Rating From BRD
            //if (average == 0 && isNoException)
            //{
            //    ratings = 5;
            //}
            //if (average == 0 && !isNoException)
            //{
            //    ratings = 4;
            //}
            //if (average >= 1 && average <= 2)
            //{
            //    ratings = 3;
            //}
            //if (average >= 3 && average <= 6)
            //{
            //    ratings = 2;
            //}
            //if (average >= 6)
            //{
            //    ratings = 1;
            //}
            #endregion

            #region New Rating From Ms.Alyn
            if (average == 0)
            {
                ratings = 4;
            }
            if (average >= 1 && average <= 3)
            {
                ratings = 4;
            }
            if (average >= 4 && average <= 6)
            {
                ratings = 3;
            }
            if (average >= 7 && average <= 9)
            {
                ratings = 2;
            }
            if (average >= 10)
            {
                ratings = 1;
            }
            #endregion

            return ratings;
        }
    }
}
