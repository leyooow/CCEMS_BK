using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.ExceptionsMgmt;
using Application.Models.DTOs.Miscellaneous;
using Application.Models.DTOs.Monetary;
using Application.Models.DTOs.SubExceptions;
using Application.Models.Helpers;
using Application.Models.Responses;
using Application.Services.Application.Services;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories
{
    public class SubExceptionsRepository : BaseRepository<ExceptionCodeRev>, ISubExceptionsRepository
    {
        private readonly CcemQatContext _context;
        private Logs _auditlogs;
        private readonly IUserClaimsService _userClaimsService;
        private readonly string UserLoginName;
        private readonly IConfiguration _config;
        private DateTime sibsDate = DateTime.Now;
        private Notification _emailNotif;
        public SubExceptionsRepository(CcemQatContext context, Logs auditLogs, IUserClaimsService userClaimsService, IConfiguration config, Notification emailNotif) : base(context)
        {
            _context = context; 
            _auditlogs = auditLogs;
            _userClaimsService = userClaimsService;
            UserLoginName = _userClaimsService.GetClaims().LoginName;
            _config = config;
            _emailNotif = emailNotif;
        }
        public async Task<List<ExceptionCodeRev>> GetSubExceptionsLists(int? pageNumber = 1, int? pageSize = 10, string? searchString = null, string? currentFilter = null)
        {
            try
            {
                var userDetails = _userClaimsService.GetClaims();
                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                List<string> branchAccess = (from x in _context.BranchAccesses
                                             where x.EmployeeId == userDetails.EmployeeID
                                             select x.BranchId.ToString()).ToList();
                var gtest = _context.ExceptionCodeRevs;
                var exceptionCodeRevs = _context.ExceptionCodeRevs;
                var exItem = _context.ExceptionItems;

                var query = from x in exceptionCodeRevs
                            join z in exItem
                            on x.ExItemRefNo equals z.RefNo
                            where x.IsProcessed == false && x.ApprovalStatus == (int)ApprovalStatusDTO.PendingApproval && branchAccess.Contains(z.BranchCode)
                            select x;

                if (!String.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();

                    query = query.Where(s =>
                    s.SubReferenceNo.ToString().ToLower().Contains(searchString)).OrderByDescending(s => s.DateCreated);
                }

                if (pageNumber.HasValue && pageSize.HasValue)
                {
                    query = query
                        .Skip((pageNumber.Value - 1) * pageSize.Value)
                        .Take(pageSize.Value);
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public async Task<SubExceptionsDetailsDTO> GetSubExceptionDetails(string subERN)
        {
            try
            {
                SubExceptionsDetailsDTO vm = new SubExceptionsDetailsDTO();
                // Retrieve sub-exception data
                var subException = await _context.ExceptionCodes.Where(s => s.SubReferenceNo == subERN).SingleOrDefaultAsync();
                if (subException == null)
                {
                    //return NotFound();
                }
                // Display equivalent devcat and risk class
                var deviationLookup = await _context.Deviations.Where(s => s.Id == subException.ExCode).SingleOrDefaultAsync();
                vm.DeviationCategory = deviationLookup.Category;
                vm.RiskClassification = deviationLookup.RiskClassification;
                //comment by James 12-18-2024
                subException.ExCodeDescription = deviationLookup.Deviation1;
                subException.TaggingDate = subException.TaggingDate?.Date;
                vm.ExceptionCode = subException;

                // Fetch exception revisions if any
                var subExceptionRevs = await _context.ExceptionCodeRevs
                    .Where(s => s.SubReferenceNo == subERN && s.IsProcessed == false)
                    .SingleOrDefaultAsync();

                if (subExceptionRevs != null)
                {
                    vm.NewStatus = (DeviationStatusDTO)subExceptionRevs.DeviationStatus;
                    vm.Remarks = subExceptionRevs.ApprovalRemarks;
                    vm.TaggingDate = subExceptionRevs.TaggingDate.Date;
                }

                // Fetch all branch replies
                var branchReplies = await _context.BranchReplies
                    .Where(s => s.ExceptionNo == subERN)
                    .OrderBy(s => s.DateCreated)
                    .ToListAsync();

                if (branchReplies.Any())
                {
                    List<string> bReplyDisplay = new List<string>();

                    foreach (var bReply in branchReplies)
                    {
                        // Add each reply's details to the list
                        bReplyDisplay.Add(bReply.ActionPlan);
                        bReplyDisplay.Add(bReply.CreatedBy);
                        bReplyDisplay.Add(bReply.DateCreated?.ToString("MM/dd/yyyy"));
                    }

                    vm.BranchReplyDetails = bReplyDisplay;
                }

                //TempData["ERN"] = subException.ExItemRefNo;
                //TempData["SubRef"] = vm.ExceptionCode.SubReferenceNo;


                var isForDeletionAsync = await _context.ExceptionCodeRevs.Where(x => x.SubReferenceNo == vm.ExceptionCode.SubReferenceNo)
                    .Take(1).OrderByDescending(x => x.Id).SingleAsync();
                if(isForDeletionAsync != null)
                {
                    var isForDeletion = isForDeletionAsync.Changes;
                    if (isForDeletion.Equals("Delete"))
                    {
                        vm.ForDeletion = 1;
                    }
                    else
                    {
                        vm.ForDeletion = 0;
                    }
                }
                return vm;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<GenericResponse<dynamic>> DeleteSubException(string subRefNo, string deleteSubExceptionRemarks)
        {
            try
            {
                SubExceptionsListViewDTO vm = new SubExceptionsListViewDTO();

                var subExceptionItem = await _context.ExceptionCodes.Where(s => s.SubReferenceNo == subRefNo).SingleOrDefaultAsync();
                if (String.IsNullOrEmpty(deleteSubExceptionRemarks))
                {
                    return ResponseHelper.ErrorResponse<dynamic>("Remarks field is mandatory. Please try again");
                }

                // Check if sub-exception exists
                if (subExceptionItem != null)
                {
                    // Update the status of the sub-exception to Open
                    subExceptionItem.ApprovalStatus = (int?)ApprovalStatusDTO.PendingApproval;
                }

                // Prevent Duplicate Request for sub-exceptions
                bool isExistingReq = await _context.ExceptionCodeRevs.AnyAsync(sub => sub.SubReferenceNo == subRefNo && sub.IsProcessed == false);
                if (!isExistingReq)
                {

                    // Save to ExceptionCodeRevs revisions table (history)
                    SaveToExceptionCodeRevs(vm, subRefNo, "Delete", deleteSubExceptionRemarks, subRefNo);

                    // Update the sub-exception status
                    _context.Update(subExceptionItem);

                    await _context.SaveChangesAsync();

                    return ResponseHelper.SuccessResponse<dynamic>($"Deletion of sub-exception with Sub Ref No [{subRefNo}] is now subject for approval");

                    // Email Notification for sub-exception deletion approval
                    //var url = $"{Settings.Config.HOST}/subexceptionsmgmt/approval/{subExceptionItem.SubRefNo}";
                    //List<string> recipients = GetRecipients(exceptionItem);
                    //string[] contentDetails = { exceptionItem.BranchName, exceptionItem.EmployeeID, "Delete Sub-Exception Request", subExceptionItem.SubRefNo, url };
                    //if (recipients.Count != 0)
                    //{
                    //    _emailNotif.SendApproverNotification(contentDetails, recipients);
                    //}
                }
                else
                {
                    return ResponseHelper.ErrorResponse<dynamic>("Sub-exception update did not push through to prevent duplication because someone has already edited the same sub-exception");
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<GenericResponse<dynamic>> UpdateSubException(string subRefNo, DeviationStatusDTO NewStatus, DateTime? TaggingDate, string ExItemRefNo)
        {
            try
            {
                string responsemsg = string.Empty;
                if (TaggingDate == null)
                {
                    return ResponseHelper.ErrorResponse<dynamic>("Please select a Tagging Date.");
                }

                if (IsUnchanged(subRefNo, NewStatus))
                {
                    return ResponseHelper.ErrorResponse<dynamic>("No changes has been made.");
                }
                else
                {
                    ExceptionCode exCode = new ExceptionCode();
                    ExceptionCodeRev exCodeRev = new ExceptionCodeRev();
                    bool isSuccessful = false;
                    bool skipApproval = false;
                    try
                    {
                        DateTime? taggingDate = null;
                        //Update NonRevs
                        if (TaggingDate != null)
                        {
                            taggingDate = TaggingDate;
                        }
                        exCode = await _context.ExceptionCodes.Include(s => s.ExItemRefNoNavigation).Where(s => s.SubReferenceNo == subRefNo).SingleOrDefaultAsync() ?? new();

                        // If update is Outstanding -> Regularize, No need for approval.
                        if (exCode.DeviationStatus == (int)DeviationStatusDTO.Outstanding && NewStatus == DeviationStatusDTO.Regularized)
                        {
                            if (taggingDate < DateTime.Now.Date)
                            {
                                exCode.ApprovalStatus = (int)ApprovalStatusDTO.PendingApproval;
                                skipApproval = false;
                                _context.Update(exCode);
                            }
                            else
                            {
                                exCode.ApprovalStatus = (int?)(NewStatus != DeviationStatusDTO.Outstanding ? ApprovalStatusDTO.Closed : ApprovalStatusDTO.Open);
                                exCode.DeviationStatus = (int)NewStatus;
                                exCode.TaggingDate = (DateTime)taggingDate;
                                skipApproval = true;
                            }
                        }
                        else
                        {
                            exCode.ApprovalStatus = (int?)ApprovalStatusDTO.PendingApproval;
                            _context.Update(exCode);
                        }
                        //Add Revision                                                            
                        exCodeRev.ExItemRefNo = exCode.ExItemRefNo;
                        exCodeRev.ExCode = exCode.ExCode;
                        //exCodeRev.ExCodeDescription = GetExCodeDescription(exCode.ExCode); NOT MAPPED
                        exCodeRev.SubReferenceNo = exCode.SubReferenceNo;
                        exCodeRev.DeviationStatus = (int)NewStatus;
                        exCodeRev.DateCreated = exCode.DateCreated;
                        exCodeRev.IsProcessed = skipApproval ? true : false;
                        exCodeRev.ApprovalStatus = (int)(skipApproval ? ApprovalStatusDTO.Open : ApprovalStatusDTO.PendingApproval);
                        exCodeRev.ModifiedDateTime = DateTime.Now;
                        exCodeRev.ModifiedBy = UserLoginName;
                        exCodeRev.Changes = "Update";
                        exCodeRev.TaggingDate = (DateTime)taggingDate;
                        exCodeRev.EntryDate = DateTime.Now;

                        if (skipApproval)
                        {
                            exCodeRev.ApprovalRemarks = "System - Outstanding to Regularize";
                            exCodeRev.ApprovedBy = UserLoginName;
                            exCodeRev.ActionTaken = "Approved";
                            exCodeRev.ApprovedDateTime = DateTime.Now;
                        }

                        await _context.AddAsync(exCodeRev);

                        //Prevent Duplicate Request resulting error on AOO side upon approval
                        bool isExistingReq = await _context.ExceptionCodeRevs.AnyAsync(s => s.SubReferenceNo == subRefNo && s.IsProcessed == false);
                        if (!isExistingReq)
                        {
                            await _context.SaveChangesAsync();
                            isSuccessful = true;

                            if (skipApproval)
                            {
                                responsemsg = "Sub-exception is now regularized";
                            }
                            else
                            {
                                responsemsg = "Sub-exception updated is now pending for approval";
                            }

                        }
                        else
                        {
                            return ResponseHelper.ErrorResponse<dynamic>("Sub-exception update did not push through to prevent duplication because someone has already edited the same sub-exception");
                        }

                        //Close mother ern when no remaining outstanding
                        bool hasOutstanding = await _context.ExceptionCodes.AnyAsync(s => s.ExItemRefNo == exCode.ExItemRefNo && s.DeviationStatus == (int)DeviationStatusDTO.Outstanding);

                        if (!hasOutstanding)
                        {
                            // Automatically close the mother ERN when no outstanding sub ERN left..
                            var exItem = _context.ExceptionItems.SingleOrDefault(s => s.RefNo == ExItemRefNo);

                            exItem.Status = (int)MainStatusDTO.Closed;

                            _context.Update(exItem);
                            await _context.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        return ResponseHelper.ErrorResponse<dynamic>($"Sorry. Something went wrong. Changes has been cancelled. [Inner Exception - {ex.InnerException}]");
                    }

                    if (isSuccessful && !skipApproval)
                    {
                        try
                        {
                            // Email Notification
                            var url = $"{_config["Host"]}/subexceptions/Details?subERN={subRefNo}";
                            List<string> recipients = GetRecipients(exCodeRev.ModifiedBy);
                            string[] contentDetails = { exCode.ExItemRefNoNavigation.BranchName, exCode.ExItemRefNoNavigation.EmployeeId, "Update Sub-Exception Status", subRefNo, url };
                            if (recipients.Count != 0)
                            {
                                //comment by James 12-18-2024
                                _emailNotif.SendApproverNotification(contentDetails, recipients);
                            }
                        }
                        catch (Exception ex)
                        {
                            return ResponseHelper.ErrorResponse<dynamic>($"Sorry. Something went wrong upon sending email notification. [Inner Exception - {ex.InnerException}]");
                        }
                    }
                }
                return ResponseHelper.SuccessResponse<dynamic>(responsemsg);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<GenericResponse<dynamic>> ApproveSubException(SubExceptionsDetailsDTO value)
        {
            try
            {
                bool isRequestor = false;
                var ecRevs = _context.ExceptionCodeRevs.Where(s => s.SubReferenceNo == value.ExceptionCode.SubReferenceNo && s.IsProcessed == false).SingleOrDefault();

                isRequestor = UserLoginName == ecRevs.ModifiedBy ? true : false;
                if (!isRequestor)
                {
                    ecRevs.ApprovalStatus = (int?)ApprovalStatusDTO.Open;
                    ecRevs.IsProcessed = true;
                    ecRevs.ActionTaken = "Approved";
                    ecRevs.ApprovedDateTime = DateTime.Now;
                    ecRevs.ApprovedBy = UserLoginName;
                    ecRevs.ApprovalRemarks = value.Remarks;
                    ecRevs.TaggingDate = (DateTime)value.TaggingDate;
                    _context.Update(ecRevs);

                    ExceptionCode ec = new ExceptionCode();
                    ec = await _context.ExceptionCodes.Where(s => s.SubReferenceNo == value.ExceptionCode.SubReferenceNo).SingleOrDefaultAsync() ?? new();
                    ec.ApprovalStatus = (int?)(value.NewStatus != DeviationStatusDTO.Outstanding ? ApprovalStatusDTO.Closed : ApprovalStatusDTO.Open);
                    ec.DeviationStatus = (int)value.NewStatus;
                    ec.ApprovalRemarks = value.Remarks;
                    ec.TaggingDate = (DateTime)value.TaggingDate;
                    _context.Update(ec);

                    await _context.SaveChangesAsync();

                    try
                    {
                        var isForDelete = _context.ExceptionCodeRevs.Where(x =>
                        x.SubReferenceNo == value.ExceptionCode.SubReferenceNo && x.ActionTaken == "Approved").Take(1).OrderByDescending(x => x.Id)
                        .Single().Changes;
                        if (isForDelete.Equals("Delete"))
                        {
                            await ApproveDelete(value.ExceptionCode.SubReferenceNo);
                        }
                    }
                    catch (Exception)
                    {
                        throw new NotImplementedException();
                    }

                    bool hasOutstanding = await _context.ExceptionCodes.AnyAsync(s => s.ExItemRefNo == value.ExceptionCode.ExItemRefNo
                        && s.DeviationStatus == (int)DeviationStatusDTO.Outstanding);

                    if (!hasOutstanding)
                    {
                        // Automatically close the mother ERN when no outstanding sub ERN left..
                        var exItem = await _context.ExceptionItems.Where(s => s.RefNo == value.ExceptionCode.ExItemRefNo).SingleOrDefaultAsync();

                        exItem.Status = (int)MainStatusDTO.Closed;

                        _context.Update(exItem);
                        await _context.SaveChangesAsync();
                    }
                    return ResponseHelper.SuccessResponse<dynamic>($"Update for Sub-Exception {value.ExceptionCode.SubReferenceNo} successfully approved");
                }
                else
                {
                    return ResponseHelper.ErrorResponse<dynamic>("You cannot approve/reject your own request. Ask for other AOO's approval");
                }

            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<GenericResponse<dynamic>> RejectSubException(SubExceptionsDetailsDTO value, string? remarks)
        {
            try
            {
                bool isRequestor = false;
                ExceptionCodeRev ecRevs = new ExceptionCodeRev();
                ecRevs = await _context.ExceptionCodeRevs.Where(s => s.SubReferenceNo == value.ExceptionCode.SubReferenceNo && s.IsProcessed == false).SingleOrDefaultAsync();
                if (remarks == null)
                {
                    return ResponseHelper.ErrorResponse<dynamic>("Remarks field cannot be empty. Please try again");
                }

                isRequestor = UserLoginName == ecRevs.ModifiedBy ? true : false;
                if (!isRequestor)
                {
                    ecRevs.ApprovalStatus = (int?)ApprovalStatusDTO.Open;
                    ecRevs.IsProcessed = true;
                    ecRevs.ActionTaken = "Rejected";
                    ecRevs.ApprovedDateTime = DateTime.Now;
                    ecRevs.ApprovedBy = UserLoginName;
                    ecRevs.ApprovalRemarks = value.Remarks;
                    _context.Update(ecRevs);

                    ExceptionCode ec = new ExceptionCode();
                    ec = await _context.ExceptionCodes.Where(s => s.SubReferenceNo == value.ExceptionCode.SubReferenceNo).SingleOrDefaultAsync();
                    ec.ApprovalStatus = (int?)ApprovalStatusDTO.Open;
                    ec.ApprovalRemarks = value.Remarks;
                    _context.Update(ec);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    return ResponseHelper.ErrorResponse<dynamic>("You cannot approve/reject your own request. Ask for other AOO's approval");
                }
                return ResponseHelper.SuccessResponse<dynamic>($"Update for Sub-Exception {value.ExceptionCode.SubReferenceNo} successfully rejected");
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public async Task<int> GetTotalCountAsync(string? searchTerm)
        {

            return await base.GetTotalCountAsync(searchTerm);
        }
        public async Task ApproveDelete(string SubRefNo)
        {
            try
            {

                var ec = new List<ExceptionCode>();
                ec = await _context.ExceptionCodes.Where(s => s.SubReferenceNo == SubRefNo).ToListAsync();
                foreach (var item in ec)
                {
                    ExceptionCode i = new ExceptionCode();
                    i = await _context.ExceptionCodes.Where(s => s.Id == item.Id).SingleAsync();

                    _context.Remove(i);
                    await _context.SaveChangesAsync();
                }

                var ecr = new List<ExceptionCodeRev>();
                ecr = _context.ExceptionCodeRevs.Where(s => s.SubReferenceNo == SubRefNo).ToList();
                foreach (var item in ecr)
                {
                    ExceptionCodeRev i = new ExceptionCodeRev();
                    i = await _context.ExceptionCodeRevs.Where(s => s.Id == item.Id).SingleAsync();

                    _context.Remove(i);
                    await _context.SaveChangesAsync();
                }

                var rpt = new List<ReportContent>();
                rpt = await _context.ReportContents.Where(s => s.ExceptionNo == SubRefNo).ToListAsync();
                foreach (var item in rpt)
                {
                    ReportContent i = new ReportContent();
                    i = await _context.ReportContents.Where(s => s.Id == item.Id).SingleAsync();

                    _context.Remove(i);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ERR)
            {

            }
        }
        public async  Task<List<ExceptionCodeRev>> GetSubExceptionsForApprovalList(int? pageNumber = 1, int? pageSize = 10, string? searchString = null, string? currentFilter = null)
        {
            try
            {
                ExceptionCodeRev exCodeRev = new ExceptionCodeRev();
                PaginatedList<ExceptionCodeRev> exCodeRevs;
                var userDetails = _userClaimsService.GetClaims();
                string loggedID = userDetails.EmployeeID;
                string loggedUser = UserLoginName;
                string loggedRole = userDetails.RoleName;

                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                List<string> branchAccess = (from x in _context.BranchAccesses
                                             where x.EmployeeId == loggedID
                                             select x.BranchId.ToString()).ToList();
                var gtest = _context.ExceptionCodeRevs;
                var exceptionCodeRevs = _context.ExceptionCodeRevs;
                var exItem = _context.ExceptionItems;

                var query = from x in exceptionCodeRevs
                            join z in exItem
                            on x.ExItemRefNo equals z.RefNo
                            where x.IsProcessed == false && x.ApprovalStatus == (int)ApprovalStatusDTO.PendingApproval && branchAccess.Contains(z.BranchCode)
                            select x;
                if (!String.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();

                    query = query.Where(s =>
                    s.SubReferenceNo.ToString().ToLower().Contains(searchString)).OrderByDescending(s => s.DateCreated);
                }
                if (pageNumber.HasValue && pageSize.HasValue)
                {
                    query = query
                        .Skip((pageNumber.Value - 1) * pageSize.Value)
                        .Take(pageSize.Value);
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        private void SaveToExceptionCodeRevs(SubExceptionsListViewDTO vm, string refNo, string changes, string remarks, string subRefNo)
        {
            ExceptionItemRevsDTO exceptionItemRevs = new ExceptionItemRevsDTO();
            #region Sub-Exceptions Logic
            // Record selected exception/s..
            var exceptionCodeList = new List<ExceptionCodeRevsDTO>();


            char endValue = '@';
            string subRef = string.Empty;

            if (changes == "Delete")
            {
                var exCodes = _context.ExceptionCodes
                        .Where(s => s.SubReferenceNo == subRefNo);

                foreach (var item in exCodes)
                {
                    var exceptionCodeRevs = new ExceptionCodeRevsDTO();
                    exceptionCodeRevs.SubReferenceNo = item.SubReferenceNo;
                    exceptionCodeRevs.ExCode = item.ExCode;
                    exceptionCodeRevs.ExItemRefNo = item.ExItemRefNo;
                    exceptionCodeRevs.DeviationStatus = (DeviationStatusDTO)item.DeviationStatus;
                    exceptionCodeRevs.ApprovalStatus = (ApprovalStatusDTO)item.ApprovalStatus;
                    exceptionCodeRevs.DateCreated = item.DateCreated;
                    exceptionCodeRevs.ModifiedBy = UserLoginName;
                    exceptionCodeRevs.EntryDate = (DateTime)item.EntryDate;
                    exceptionCodeRevs.ApprovalRemarks = item.ApprovalRemarks;
                    exceptionCodeRevs.IsProcessed = false;
                    exceptionCodeRevs.Changes = "Delete";
                    exceptionCodeRevs.ModifiedDateTime = DateTime.Now;
                    exceptionCodeList.Add(exceptionCodeRevs);

                    _context.Update(exceptionCodeRevs);

                    _context.SaveChangesAsync();
                }
            }

            #endregion

        }
        private bool IsUnchanged(string subRef, DeviationStatusDTO status)
        {
            bool unchanged = false;

            var model = _context.ExceptionCodes.SingleOrDefault(s => s.SubReferenceNo == subRef);

            unchanged = model.DeviationStatus == (int)status;

            return unchanged;
        }
        private string GetExCodeDescription(int exCode)
        {
            string desc;
            var deviationLookup = _context.Deviations.SingleOrDefault(s => s.Id == exCode);
            desc = deviationLookup.Deviation1;

            return desc;
        }
        private List<string> GetRecipients(string createdBy)
        {
            List<string> recipients = new List<string>();
            List<string> employeesIDs = new List<string>();
            List<int> branchIDs = new List<int>();

            //string createdBy = query.CreatedBy;

            var assignedBranches = _context.Users.Include(s => s.BranchAccesses).SingleOrDefault(s => s.LoginName == createdBy).BranchAccesses.ToList();

            foreach (var item in assignedBranches)
            {
                employeesIDs.Add(item.EmployeeId);
                branchIDs.Add(item.BranchId);
            }
            //use branch id to identify the users with branch coverage
            var matchingIDs = _context.BranchAccesses.Where(s => branchIDs.Contains(s.BranchId)).Select(s => s.EmployeeId);
            //use the collected id/s to find the matched users 
            var matchingPeople = _context.Users.Where(x => matchingIDs.Contains(x.EmployeeId));

            if (matchingPeople != null)
            {
                if (matchingPeople.Count() != 0)
                {
                    recipients = matchingPeople.Where(s => s.RoleId == 3 && s.RoleId == 5).Select(s => s.Email).ToList();
                }
            }

            return recipients;
        }
    }
}
