using Application.Contracts.Repositories;
using Application.Models.DTOs.Deviation;
using Application.Contracts.Services;
using Application.Models.DTOs.ExceptionsMgmt;
using Application.Models.DTOs.Miscellaneous;
using Application.Models.DTOs.Monetary;
using Application.Models.DTOs.Report;
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
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories
{
    public class ExceptionsMgmtRepository : BaseRepository<ExceptionItem>, IExceptionsMgmtRepository
    {
        private readonly CcemQatContext _context;
        private Logs _auditlogs;
        private readonly IUserClaimsService _userClaimsService;
        private readonly string UserLoginName;
        private readonly IConfiguration _config;
        private DateTime sibsDate = DateTime.Now;
        private Notification _emailNotif;

//         public ExceptionsMgmtRepository(CcemQatContext context, Logs auditLogs, UserClaimsService userClaimsService, IConfiguration config, Notification emailNotif) : base(context)

        public ExceptionsMgmtRepository (CcemQatContext context, Logs auditLogs, IUserClaimsService userClaimsService, IConfiguration config, Notification emailNotif) : base(context)

        {
            _context = context;
            _auditlogs = auditLogs;
            _userClaimsService = userClaimsService;
            UserLoginName = _userClaimsService.GetClaims().LoginName;
            _config = config;
            _emailNotif = emailNotif;
        }
        public async Task<List<ExceptionItem>> GetExceptionsList(int? pageNumber = 1, int? pageSize = 10, string? searchTerm = null, int? status = null)
        {
            try
            {
                var userDetail = _userClaimsService.GetClaims();
                var loggedRole = userDetail.RoleName;
                List<string> branchAccess = (from x in _context.BranchAccesses
                                             where x.EmployeeId == userDetail.EmployeeID
                                             select x.BranchId.ToString()).ToList();

                var query = from x in _context.ExceptionItems.Include(s => s.ExceptionCodes)
                            where branchAccess.Contains(x.BranchCode)
                            select x;

                if (_config["EnableAllViewingOfClosedException"].Equals("0"))
                {
                    if (loggedRole == "BCO" || loggedRole == "BCA")
                    {
                        //TF Reference No - NL-091321-008
                        //AOO raised findings where BOCC should only view their own created exceptions only. 
                        query = query.Where(s => s.CreatedBy == UserLoginName);
                    }
                }

                MainStatusDTO mainStatus = MainStatusDTO.Default;
                if (status != null)
                {
                    mainStatus = (MainStatusDTO)status;
                }

                if (mainStatus != MainStatusDTO.Default)
                {
                    query = from x in query
                            where x.Status == status
                            orderby x.EntryDate descending
                            select x;
                }

                if (!String.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(s => s.RefNo.ToString().ToLower().Contains(searchTerm)).OrderByDescending(s => s.EntryDate);
                }

                query = query.OrderByDescending(s => s.EntryDate);
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
                throw new NotImplementedException();
            }
        }
        public async Task<ExceptionViewDTO> GetExceptionDetails(string id)
        {
            try
            {
                var obj = new ExceptionViewDTO();
                obj.HasPendingUpdate = false;
                if (id != null)
                {
                    string type = string.Empty;

                    var exceptionItem = new ExceptionItem();
                    var exceptionItemRevs = new ExceptionItemRevsDTO();
                    ICollection<ExceptionCodeRevsDTO> exCodeRevsCollection = new List<ExceptionCodeRevsDTO>();
                    TransactionTypeEnumDTO tranType = new TransactionTypeEnumDTO();

                    exceptionItem = await _context.ExceptionItems
                    .Include(s => s.ExceptionCodes)
                    .Include(s => s.Monetaries)
                    .Include(s => s.NonMonetaries)
                    .Include(s => s.Miscs)
                    .FirstOrDefaultAsync(m => m.RefNo == id);

                    exceptionItem.Monetaries = [.. exceptionItem.Monetaries.OrderByDescending(m => m.Id)];
                    exceptionItem.Miscs = [.. exceptionItem.Miscs.OrderByDescending(m => m.Id)];
                    exceptionItem.NonMonetaries = [.. exceptionItem.NonMonetaries.OrderByDescending(m => m.Id)];

                    tranType = (TransactionTypeEnumDTO)exceptionItem.Type;
                    obj.ExceptionItem = exceptionItem;

                    int excode = exceptionItem.ExceptionCodes.FirstOrDefault().ExCode;
                    var getDeviationStatus = _context.ExceptionCodeRevs.Where(a => a.ExCode == excode).OrderByDescending(x => x.EntryDate).FirstOrDefault().DeviationStatus;


                    switch (tranType)
                    {
                        case TransactionTypeEnumDTO.Monetary:
                            type = "Monetary";
                            break;
                        case TransactionTypeEnumDTO.NonMonetary:
                            type = "Non-Monetary";
                            break;
                        case TransactionTypeEnumDTO.Miscellaneous:
                            type = "Miscellaneous";
                            break;
                        default:
                            break;
                    }

                    var deviations = from s in _context.Deviations
                                     where s.Classification == type
                                     select s;

                    List<SubExceptionsListViewDTO> subExceptionList = new List<SubExceptionsListViewDTO>();

                    foreach (var item in exceptionItem.ExceptionCodes)
                    {
                        SubExceptionsListViewDTO subException = new SubExceptionsListViewDTO();

                        var devLookup = _context.Deviations.SingleOrDefault(s => s.Id == item.ExCode);


                        subException.SubReferenceNo = item.SubReferenceNo;
                        subException.ExItemRefNo = item.ExItemRefNo;
                        subException.DateCreated = item.DateCreated;
                        subException.ApprovalStatus = (ApprovalStatusDTO)item.ApprovalStatus;
                        subException.DeviationStatus = (DeviationStatusDTO)item.DeviationStatus;
                        subException.ExCode = item.ExCode;
                        subException.ExCodeDescription = devLookup.Deviation1;
                        subException.DeviationCategory = devLookup.Category;
                        subException.RiskClassification = devLookup.RiskClassification;
                        subExceptionList.Add(subException);

                    }
                    obj.SubExceptionItems = subExceptionList;

                    obj.HasPendingUpdate = subExceptionList.Any(s => s.ApprovalStatus == ApprovalStatusDTO.PendingApproval);

                    //var revs = _context.ExceptionItemRevs.LastOrDefault(s => s.RefNo == id && !s.IsProcessed);
                    var revs = _context.ExceptionItemRevs
                               .Where(s => s.RefNo == id && !s.IsProcessed)
                               .OrderByDescending(s => s.Id)
                               .LastOrDefault();


                    
                    if (revs != null)
                    {
                        obj.Request = revs.Changes;
                    }

                    PopulateRiskClassificationDropDownList(exceptionItem.RiskClassificationId);
                    PopulateDeviationCategoryDropDownList(exceptionItem.DeviationCategoryId);

                    if (obj == null)
                    {
                        throw new NotImplementedException();
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<ExceptionViewDTO> GetExceptionUpdateDetails(string id)
        {
            try
            {
                var obj = new ExceptionViewDTO();
                obj.HasPendingUpdate = false;
                if (id != null)
                {
                    string type = string.Empty;

                    var exceptionItem = new ExceptionItem();
                    var exceptionItemRevs = new ExceptionItemRevsDTO();
                    ICollection<ExceptionCode> exCodeCollection = new List<ExceptionCode>();
                    ICollection<ExceptionCodeRevsDTO> exCodeRevsCollection = new List<ExceptionCodeRevsDTO>();
                    TransactionTypeEnumDTO tranType = new TransactionTypeEnumDTO();

                    exceptionItem = await _context.ExceptionItems
                           .Include(s => s.ExceptionCodes)
                           .Include(s => s.Monetaries)
                           .Include(s => s.NonMonetaries)
                           .Include(s => s.Miscs)
                           .FirstOrDefaultAsync(m => m.RefNo == id);


                    exceptionItem.Monetaries = [.. exceptionItem.Monetaries.OrderByDescending(m => m.Id)];
                    exceptionItem.Miscs = [.. exceptionItem.Miscs.OrderByDescending(m => m.Id)];
                    exceptionItem.NonMonetaries = [.. exceptionItem.NonMonetaries.OrderByDescending(m => m.Id)];
                    //if (exceptionItem.Status == MainStatus.Closed)
                    //{
                    //    return("You are automatically redirected to Dashboard due to selected Exception is already Closed and cannot be modified.");
                    //    return RedirectToAction("Index");
                    //}

                    tranType = (TransactionTypeEnumDTO)exceptionItem.Type;
                    exCodeCollection = exceptionItem.ExceptionCodes;
                    obj.ExceptionItem = exceptionItem;

                    switch (tranType)
                    {
                        case TransactionTypeEnumDTO.Monetary:
                            type = "Monetary";
                            break;
                        case TransactionTypeEnumDTO.NonMonetary:
                            type = "Non-Monetary";
                            break;
                        case TransactionTypeEnumDTO.Miscellaneous:
                            type = "Miscellaneous";
                            break;
                        default:
                            break;
                    }

                    var deviations = from s in _context.Deviations
                                     where s.Classification == type
                                     select s;


                    List<int> intList = new List<int>();

                    if (exCodeCollection.Count != 0)
                    {
                        foreach (var item in exCodeCollection)
                        {
                            if ((DeviationStatus)item.DeviationStatus != DeviationStatus.Outstanding)
                            {
                                continue;
                            }

                            int exCode = 0;
                            exCode = item.ExCode;
                            intList.Add(exCode);

                        }
                        obj.SelectedExCodes = intList;
                    }

                    //var revs = _context.ExceptionItemRevs.LastOrDefault(s => s.RefNo == id && !s.IsProcessed);
                    var revs = _context.ExceptionItemRevs
                               .Where(s => s.RefNo == id && !s.IsProcessed)
                               .OrderByDescending(s => s.Id)
                               .LastOrDefault();
                    if (revs != null)
                    {
                        obj.Request = revs.Changes;
                    }

                    PopulateRiskClassificationDropDownList(exceptionItem.RiskClassificationId);
                    PopulateDeviationCategoryDropDownList(exceptionItem.DeviationCategoryId);

                    if (obj == null)
                    {
                        throw new NotImplementedException();
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<string> SaveException(ExceptionViewDTO value)
        {
            try
            {
                TransactionTypeEnumDTO type = (TransactionTypeEnumDTO)value.ExceptionItemRevs.Type;
                NullUnselectedTypesRevs(type, value);
                string branchCode = value.ExceptionItemRevs.BranchCode;
                string refNo = GenerateReferenceNo(type, branchCode);
                SaveToExceptionItemRevs(value, type, refNo, "Create", string.Empty);
                SaveToExceptionItem(value, type, refNo);
                return refNo;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<string> UpdateException(ExceptionViewDTO value)
        {
            try
            {
                // Create New ExceptionItem Revision. Supply data from View Model.
                SaveToExceptionItemRevs(value, (TransactionTypeEnumDTO)value.ExceptionItem.Type, value.ExceptionItem.RefNo, "Update", "");

                // Tag the Main Exception/ Non-Revision as Pending if above is successful.
                var exceptionItem = await _context.ExceptionItems.SingleOrDefaultAsync(s => s.RefNo == value.ExceptionItem.RefNo && s.Id == value.ExceptionItem.Id);
                exceptionItem.Status = (int)MainStatusDTO.PendingApproval;
                _context.Update(exceptionItem);

                //Prevent Duplicate Request resulting error on AOO side upon approval
                bool isExistingReq = await _context.ExceptionItemRevs.AnyAsync(s => s.RefNo == value.ExceptionItem.RefNo && s.IsProcessed == false);
                if (!isExistingReq)
                {
                    _context.SaveChanges();
                    // Email Notification
                    //var encodedUrl = HttpContext.Request.GetEncodedUrl();
                    var url = $"{_config["Host"]}/exceptionsmgmt/details/{value.ExceptionItem.RefNo}";
                    string[] contentDetails = { exceptionItem.BranchName, exceptionItem.EmployeeId, "Update Exception Details", exceptionItem.RefNo, url };
                    List<string> recipients = GetRecipients(exceptionItem);
                    if (recipients.Count != 0)
                    {
                        _emailNotif.SendApproverNotification(contentDetails, recipients);
                    }

                    return string.Format("Updating request of exception with Ref No [{0}] is now subject for approval", value.ExceptionItem.RefNo);
                }
                else
                {
                    return "Exception update did not push through to prevent duplication because someone has already edited the same sub-exception";
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<int> GetTotalCountAsync(string? searchTerm)
        {
            return await base.GetTotalCountAsync(searchTerm);
        }
        public async Task<List<Deviation>> GetDeviationByClasification(string classification)
        {
            return await _context.Deviations
                .Where(d => d.Classification == classification)
                .ToListAsync();
        }
        public async Task<List<Deviation>> GetDeviationByIdsAsync(int[] ids)
        {
            return await _context.Deviations
                .Where(d => ids.Contains(d.Id))
                .ToListAsync();
        }

        public async Task<List<ExceptionItemRev>> GetExceptionsForApprovalList(int? pageNumber = 1, int? pageSize = 10, string? searchString = null, string? currentFilter = null)
        {
            try
            {
                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                var userDetails = _userClaimsService.GetClaims();
                List<string> branchAccess = (from x in _context.BranchAccesses
                                             where x.EmployeeId == userDetails.EmployeeID
                                             select x.BranchId.ToString()).ToList();

                IQueryable<ExceptionItemRev> query = from x in _context.ExceptionItemRevs
                                                     where x.Status == (int)MainStatusDTO.PendingApproval &&
                                                           x.IsProcessed == false &&
                                                           branchAccess.Contains(x.BranchCode)
                                                     select new ExceptionItemRev
                                                     {
                                                         Id = x.Id,
                                                         RefNo = x.RefNo, // Include the RefNo property
                                                         ModifiedBy = x.ModifiedBy ?? null,
                                                         ModifiedDateTime = x.ModifiedDateTime ?? DateTime.MinValue,
                                                         ApprovedBy = x.ApprovedBy ?? null,
                                                         ApprovedDateTime = x.ApprovedDateTime ?? DateTime.MinValue,
                                                         // Include default values for other fields,
                                                         EmployeeId = x.EmployeeId,
                                                         BranchCode = x.BranchCode,
                                                         BranchName = x.BranchName,
                                                         Type = x.Type,
                                                         TransactionDate = x.TransactionDate,
                                                         CreatedBy = x.CreatedBy,
                                                         Severity = x.Severity,
                                                         Status = x.Status,
                                                         DateCreated = x.DateCreated,
                                                         Changes = x.Changes
                                                     };
                if (!String.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    query = query.Where(s =>
                        s.RefNo.ToString().ToLower().Contains(searchString.ToLower()));
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public async Task<ExceptionViewDTO> GetExceptionsForApprovalDetails(string id)
        {
            try
            {
                var exVM = new ExceptionViewDTO();
                exVM.HasPendingUpdate = false;
                if (id != null)
                {
                    string type = string.Empty;

                    var exceptionItem = new ExceptionItem();
                    var exceptionItemRevs = new ExceptionItemRev();
                    TransactionTypeEnumDTO tranType = new TransactionTypeEnumDTO();

                    //Query NonRevs
                    exceptionItem = _context.ExceptionItems
                        .Include(s => s.Monetaries)
                        .Include(s => s.NonMonetaries)
                        .Include(s => s.Miscs)
                        .Where(s => s.RefNo == id).SingleOrDefault();

                    if (exceptionItem != null)
                    {
                        exVM.ExceptionItem = exceptionItem;
                    }

                    //Start of debugging
                    var query = _context.ExceptionItemRevs
                                .Include(s => s.ExceptionCodeRevs)
                                .Include(s => s.MonetaryRevs)
                                .Include(s => s.NonMonetaryRevs)
                                .Include(s => s.MiscRevs)
                                .Where(s => s.RefNo == id && !s.IsProcessed)
                                .OrderByDescending(s => s.DateCreated);

                    if (exceptionItemRevs != null)
                    {

                        // Assign default values to properties that might be null
                        //comment by James 12-19-2024
                        //var actionPlan = exceptionItemRevs.ActionPlan ?? null;
                        var actionTaken = exceptionItemRevs.ActionTaken ?? null;
                        var age = exceptionItemRevs.Age != null ? (int?)exceptionItemRevs.Age : null;
                        var agingCategory = exceptionItemRevs.AgingCategory != null ? (int?)exceptionItemRevs.AgingCategory : null;
                        var approvalRemarks = exceptionItemRevs.ApprovalRemarks ?? null;
                        var approvedBy = exceptionItemRevs.ApprovedBy ?? null;
                        var approvedDateTime = exceptionItemRevs.ApprovedDateTime ?? null;
                        var area = exceptionItemRevs.Area ?? null;
                        var branchCode = exceptionItemRevs.BranchCode ?? null;
                        var branchName = exceptionItemRevs.BranchName ?? null;
                        var changes = exceptionItemRevs.Changes ?? null;
                        var createdBy = exceptionItemRevs.CreatedBy ?? null;
                        var dateCreated = exceptionItemRevs.DateCreated != null ? (DateTime?)exceptionItemRevs.DateCreated : null;
                        var devApprovedBy = exceptionItemRevs.DeviationApprovedBy ?? null;
                        var devApprover = exceptionItemRevs.DeviationApprover ?? null;
                        var devCatId = exceptionItemRevs.DeviationCategoryId != null ? (int?)exceptionItemRevs.DeviationCategoryId : null;
                        var division = exceptionItemRevs.Division ?? null;
                        var empId = exceptionItemRevs.EmployeeId ?? null;
                        //DateTime? entryDate = (DateTime?)exceptionItemRevs.EntryDate ?? null;
                        var entryDate = exceptionItemRevs.EntryDate != null ? exceptionItemRevs.EntryDate : DateTime.MinValue;
                        var exCodeRevs = exceptionItemRevs.ExceptionCodeRevs ?? null;
                        var Id = exceptionItemRevs.Id != null ? (Guid?)exceptionItemRevs.Id : null;
                        var isCredit = exceptionItemRevs.IsCredit != null ? (bool?)exceptionItemRevs.IsCredit : null;
                        var isProcessed = exceptionItemRevs.IsProcessed != null ? (bool?)exceptionItemRevs.IsProcessed : null;
                        var miscRevs = exceptionItemRevs.MiscRevs ?? null;
                        var modifiedBy = exceptionItemRevs.ModifiedBy ?? null;
                        var modifiedDateTime = exceptionItemRevs.ModifiedDateTime ?? null;
                        var monetaryRevs = exceptionItemRevs.MonetaryRevs ?? null;
                        var nonMonetaryRevs = exceptionItemRevs.NonMonetaryRevs ?? null;
                        var otherPersonResponsible = exceptionItemRevs.OtherPersonResponsible ?? null;
                        var otherRemarks = exceptionItemRevs.OtherRemarks ?? null;
                        var personResponsible = exceptionItemRevs.PersonResponsible ?? null;
                        var redFlag = exceptionItemRevs.RedFlag != null ? (bool?)exceptionItemRevs.RedFlag : null;
                        var refNo = exceptionItemRevs.RefNo ?? null;
                        var remarks = exceptionItemRevs.Remarks ?? null;
                        var riskClassificationId = exceptionItemRevs.RiskClassificationId != null ? (int?)exceptionItemRevs.RiskClassificationId : null;
                        var rootCause = exceptionItemRevs.RootCause != null ? (RootCauseDTO?)exceptionItemRevs.RootCause : null;
                        var severity = exceptionItemRevs.Severity != null ? (SeverityDTO?)exceptionItemRevs.Severity : null;
                        var status = exceptionItemRevs.Status != null ? (MainStatusDTO?)exceptionItemRevs.Status : null;
                        var transDate = exceptionItemRevs.TransactionDate != null ? (DateTime?)exceptionItemRevs.TransactionDate : null;
                    }

                    exceptionItemRevs = query.LastOrDefault();
                    //End of debugging

                    if (exceptionItemRevs != null)
                    {
                        exVM.Request = exceptionItemRevs.Changes;
                        exVM.ExceptionItemRevs = exceptionItemRevs;
                    }

                    tranType = (TransactionTypeEnumDTO)exceptionItemRevs.Type;


                    switch (tranType)
                    {
                        case TransactionTypeEnumDTO.Monetary:
                            type = "Monetary";
                            break;
                        case TransactionTypeEnumDTO.NonMonetary:
                            type = "Non-Monetary";
                            break;
                        case TransactionTypeEnumDTO.Miscellaneous:
                            type = "Miscellaneous";
                            break;
                        default:
                            break;
                    }

                    var deviations = from s in _context.Deviations
                                     where s.Classification == type
                                     select s;

                    List<SubExceptionsListViewDTO> subExceptionList = new List<SubExceptionsListViewDTO>();


                    foreach (var item in exceptionItemRevs.ExceptionCodeRevs)
                    {
                        SubExceptionsListViewDTO subException = new SubExceptionsListViewDTO();

                        var deviationLookup = _context.Deviations.SingleOrDefault(s => s.Id == item.ExCode);

                        subException.SubReferenceNo = item.SubReferenceNo;
                        subException.ExItemRefNo = item.ExItemRefNo;
                        subException.DateCreated = (DateTime)item.DateCreated;
                        subException.ApprovalStatus = (ApprovalStatusDTO)item.ApprovalStatus;
                        subException.DeviationStatus = (DeviationStatusDTO)item.DeviationStatus;
                        subException.ExCode = item.ExCode;
                        subException.ExCodeDescription = deviationLookup.Deviation1;
                        subException.DeviationCategory = deviationLookup.Category;
                        subException.RiskClassification = deviationLookup.RiskClassification;
                        subException.Request = item.Changes;
                        subExceptionList.Add(subException);
                        //GetExCodeDescription();
                    }
                    exVM.SubExceptionItems = subExceptionList;

                    exVM.HasPendingUpdate = subExceptionList.Any(s => s.ApprovalStatus == ApprovalStatusDTO.PendingApproval);
                    PopulateRiskClassificationDropDownList(exceptionItemRevs.RiskClassificationId);
                    PopulateDeviationCategoryDropDownList(exceptionItemRevs.DeviationCategoryId);
                }
                return exVM;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public async Task<string> DeleteException(string refNo, string? remarks)
        {
            try
            {
                ExceptionViewDTO vm = new ExceptionViewDTO();
                var exceptionItem = _context.ExceptionItems
                    .Include(s => s.ExceptionCodes)
                    .Include(s => s.Monetaries)
                    .Include(s => s.NonMonetaries)
                    .Include(s => s.Miscs).SingleOrDefault(s => s.RefNo == refNo);

                if (String.IsNullOrEmpty(remarks))
                {
                    return "Remarks field is mandatory. Please try again";
                }
                List<int> exCode = new List<int>();
                if (exceptionItem != null)
                {
                    foreach (var item in exceptionItem.ExceptionCodes)
                    {
                        exCode.Add(item.ExCode);
                    }
                    vm.SelectedExCodes = exCode;
                    vm.ExceptionItem = exceptionItem;
                    vm.ExceptionItem.Status = (int)MainStatus.PendingApproval;



                    //Prevent Duplicate Request resulting error on AOO side upon approval
                    bool isExistingReq = _context.ExceptionItemRevs.Any(s => s.RefNo == vm.ExceptionItem.RefNo && s.IsProcessed == false);
                    if (!isExistingReq)
                    {
                        SaveToExceptionItemRevs(vm, (TransactionTypeEnumDTO)vm.ExceptionItem.Type, refNo, "Delete", remarks);
                        _context.Update(vm.ExceptionItem);
                        await _context.SaveChangesAsync();


                        // Email Notification
                        var url = $"{Settings.Config.HOST}/exceptionsmgmt/approval/{vm.ExceptionItem.RefNo}";
                        List<string> recipients = GetRecipients(exceptionItem);
                        string[] contentDetails = { exceptionItem.BranchName, exceptionItem.EmployeeId, "Delete Exception Request", vm.ExceptionItem.RefNo, url };
                        if (recipients.Count != 0)
                        {
                            _emailNotif.SendApproverNotification(contentDetails, recipients);
                        }
                    }
                    else
                    {
                        return string.Format("Exception update did not push through to prevent duplication because someone has already edited the same sub-exception", refNo);
                    }
                    return string.Format("Deletion of exception with Ref No [{0}] is now subject for approval", refNo);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
            return "";
        }
        public async Task<string> ApproveException(string refNo, string? remarks)
        {
            try
            {
                bool isRequestor = false;
                string loggedUser = UserLoginName;
                ExceptionViewDTO vm = new ExceptionViewDTO();
                var url = $"{_config["HOST"]}/exceptionsmgmt/details/{refNo}";
                var exItemRevs = _context.ExceptionItemRevs.Include(s => s.ExceptionCodeRevs).Include(s => s.MonetaryRevs).Include(s => s.NonMonetaryRevs).Include(s => s.MiscRevs)
                    .Where(s => s.RefNo == refNo /*&& s.Id == id*/ && s.IsProcessed == false)
                    .ToList().Take(1).SingleOrDefault();

                if (string.IsNullOrEmpty(remarks))
                {
                    remarks = string.Empty;
                }

                string message = "";
                if (exItemRevs != null)
                {
                    isRequestor = exItemRevs.ModifiedBy == loggedUser ? true : false;
                    if (!isRequestor)
                    {
                        // Note: 0 = Branch/Group, 1 = Employee ID/Name, 2 = Action, 3 = ReferenceNo
                        string[] contentDetails = { exItemRevs.BranchName, exItemRevs.EmployeeId, exItemRevs.Changes, exItemRevs.RefNo.ToUpper(), url.ToLower() };
                        string user = exItemRevs.CreatedBy;
                        var userDetails = _context.Users.SingleOrDefault(s => s.LoginName == user);

                        vm.ApprovalRemarks = remarks;

                        if (userDetails != null)
                        {
                            switch (exItemRevs.Changes)
                            {
                                case "Update":
                                    vm.ExceptionItemRevs = exItemRevs;
                                    UpdateExceptionItem(vm, "Approved");
                                    UpdateExceptionItemRevs(vm, (TransactionTypeEnum)vm.ExceptionItemRevs.Type, null, "Approved");
                                    await _context.SaveChangesAsync();
                                    message = string.Format("Exception with Ref No [{0}] has been updated.", refNo);
                                    _emailNotif.SendMakerNotification(contentDetails, userDetails.Email, true);
                                    break;
                                case "Delete":
                                    vm.ExceptionItemRevs = exItemRevs;
                                    UpdateExceptionItemRevs(vm, (TransactionTypeEnum)vm.ExceptionItemRevs.Type, null, "Approved");
                                    DeleteExceptionItem(vm, (TransactionTypeEnum)vm.ExceptionItemRevs.Type, vm.ExceptionItemRevs.RefNo);
                                    await _context.SaveChangesAsync();
                                    message = string.Format("Exception with Ref No [{0}] has been deleted.", refNo);
                                    _emailNotif.SendMakerNotification(contentDetails, userDetails.Email, true);
                                    break;
                                case "Report Sending":
                                    break;
                                case "Regularize":
                                    break;
                                default:
                                    break;
                            }
                        }
                        return message;
                    }
                    else
                    {
                        return "You cannot approve/reject your own request. Ask for other AOO's approval";
                    }
                }
                return message;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public async Task<string> RejectException(string refNo, string? remarks)
        {
            try
            {
                bool isRequestor = false;
                string loggedUser = UserLoginName;
                ExceptionViewDTO vm = new ExceptionViewDTO();
                var exItemRevs = _context.ExceptionItemRevs.Include(s => s.ExceptionCodeRevs).Include(s => s.MonetaryRevs).Include(s => s.NonMonetaryRevs).Include(s => s.MiscRevs)
                      .SingleOrDefault(s => s.RefNo == refNo /*&& s.Id == id*/ && s.IsProcessed == false);

                if (string.IsNullOrEmpty(remarks))
                {
                    remarks = string.Empty;
                    //_toastNotif.AddWarningToastMessage("Remarks field cannot be empty. Please try again");
                    //return RedirectToAction("ApprovalDetails", new { id = exItemRevs.RefNo });
                }
                var message = "";
                if (exItemRevs != null)
                {
                    isRequestor = loggedUser == exItemRevs.ModifiedBy ? true : false;

                    if (!isRequestor)
                    {
                        //exItemRevs.ApprovalRemarks = remarks;

                        // Note: 0 = Branch/Group, 1 = Employee ID/Name, 2 = Action, 3 = ReferenceNo
                        var url = $"{Settings.Config.HOST}/exceptionsmgmt/details/{refNo}";
                        string[] contentDetails = { exItemRevs.BranchName, exItemRevs.EmployeeId, exItemRevs.Changes, exItemRevs.RefNo.ToUpper(), remarks, url.ToLower() };
                        string user = exItemRevs.CreatedBy;
                        var userDetails = _context.Users.SingleOrDefault(s => s.LoginName == user);

                        vm.ApprovalRemarks = remarks;

                        if (userDetails != null)
                        {
                            switch (exItemRevs.Changes)
                            {
                                case "Update":
                                    vm.ExceptionItemRevs = exItemRevs;
                                    UpdateExceptionItemRevs(vm, (TransactionTypeEnum)vm.ExceptionItemRevs.Type, null, "Rejected");
                                    UpdateExceptionItem(vm, "Rejected");
                                    _context.SaveChanges();
                                    message = string.Format("Exception with Ref No [{0}] has been rejected.", refNo);
                                    _emailNotif.SendMakerNotification(contentDetails, userDetails.Email, false);
                                    break;
                                case "Delete":
                                    vm.ExceptionItemRevs = exItemRevs;
                                    UpdateExceptionItemRevs(vm, (TransactionTypeEnum)vm.ExceptionItemRevs.Type, null, "Rejected");
                                    UpdateExceptionItem(vm, "Rejected");
                                    _context.SaveChanges();
                                    message = string.Format("Exception with Ref No [{0}] has been rejected.", refNo);
                                    _emailNotif.SendMakerNotification(contentDetails, userDetails.Email, false);
                                    break;
                                case "Regularize":
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        message = "You cannot approve/reject your own request. Ask for other AOO's approval";
                    }
                }
                return message;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        #region PRIVATE METHODS
        private void PopulateRiskClassificationDropDownList(object selected = null)
        {
            var riskClassifications = from s in _context.RiskClassificationsLookups
                                      select s;
            //ViewBag.RiskClassifications = new SelectList(riskClassifications.AsNoTracking(), "Id", "Description", selected);
        }
        private void PopulateDeviationCategoryDropDownList(object selected = null)
        {
            var deviationCategory = from s in _context.DeviationCategoryLookups
                                    select s;
            //ViewBag.DeviationCategory = new SelectList(deviationCategory.AsNoTracking(), "Id", "Description", selected);
        }
        private void NullUnselectedTypesRevs(TransactionTypeEnumDTO type, ExceptionViewDTO vm)
        {
            if (type == TransactionTypeEnumDTO.Monetary)
            {
                vm.ExceptionItemRevs.MiscRevs = null;
                vm.ExceptionItemRevs.NonMonetaryRevs = null;
            }
            else if (type == TransactionTypeEnumDTO.NonMonetary)
            {
                vm.ExceptionItemRevs.MiscRevs = null;
                vm.ExceptionItemRevs.MonetaryRevs = null;
            }
            else
            {
                vm.ExceptionItemRevs.MonetaryRevs = null;
                vm.ExceptionItemRevs.NonMonetaryRevs = null;
            }
        }

        /// <summary>
        /// Exception Reference Number (ERN) Structure - AAABBBYYYYMMDDNNNN 
        /// Where AAA = OTC/KYC/MSC
        /// NNNN = number series
        /// BBB = branch code
        /// MMDDYYYY = date
        /// </summary>
        private string GenerateReferenceNo(TransactionTypeEnumDTO type, string branchCode)
        {
            string refNo = string.Empty;
            string typeCode = string.Empty;
            string noSeries = GetNextSeries().ToString().PadLeft(4, '0');
            string date = DateTime.Now.ToString("yyyyMMdd");
            branchCode = branchCode.PadLeft(3, '0');

            switch (type)
            {
                case TransactionTypeEnumDTO.Monetary:
                    typeCode = "OTC";
                    break;
                case TransactionTypeEnumDTO.NonMonetary:
                    typeCode = "KYC";
                    break;
                case TransactionTypeEnumDTO.Miscellaneous:
                    typeCode = "MSC";
                    break;
                default:
                    break;
            }

            refNo = string.Format("{0}{1}{2}{3}", typeCode, branchCode, date, noSeries);

            return refNo;
        }
        private int GetNextSeries()
        {
            int sequence = 1;
            var now = DateTime.Now;
            var lastSequence = _context.RefNoSequences.OrderBy(s => s.Date).LastOrDefault();

            var details = new RefNoSequence();

            if (lastSequence != null)
            {
                details.Date = DateTime.Now;

                if (lastSequence.Date.Date == now.Date)
                {
                    details.Series = lastSequence.Series + 1;
                }
                else
                {
                    details.Series = sequence;
                }
            }
            else
            {

                details.Date = DateTime.Now;
                details.Series = sequence;
            }

            _context.RefNoSequences.Add(details);
            _context.SaveChanges();

            sequence = details.Series;

            return sequence;
        }
        private void SaveToExceptionItemRevs(ExceptionViewDTO vm, TransactionTypeEnumDTO type, string refNo, string changes, string remarks)
        {
            var userDetail = _userClaimsService.GetClaims();
            ExceptionItemRev exceptionItemRevs = new ExceptionItemRev();
            // comment by james 12-18-2024 finacle connection
            //sibsDate = _sibsDA.GetSibsDate();
            sibsDate = DateTime.Now;
            if (vm.ExceptionItemRevs != null)
            {
                exceptionItemRevs = vm.ExceptionItemRevs;
            }
            #region Sub-Exceptions Logic
            // Record selected exception/s..
            var exceptionCodeList = new List<ExceptionCodeRev>();

            if (vm.SelectedExCodes != null)
            {
                char endValue = '@';
                string subRef = string.Empty;

                foreach (var item in vm.SelectedExCodes)
                {
                    bool isExist = false;

                    if (vm.ExceptionItem != null)
                    {
                        isExist = _context.ExceptionCodes
                      .Any(s => s.ExItemRefNo == vm.ExceptionItem.RefNo && s.ExCode == item);
                    }

                    if (!isExist)
                    {
                        if (changes == "Update")
                        {
                            subRef = GetNextSubSeries(vm.ExceptionItem.RefNo, endValue);
                        }
                        else
                        {
                            subRef = GetNextSubSeries(refNo, endValue);
                        }

                        //string subRefNo = GetNextSubSeries(refNo, endValue);
                        var exceptionCode = new ExceptionCodeRev();
                        exceptionCode.SubReferenceNo = subRef;
                        exceptionCode.ExCode = item;
                        exceptionCode.ExItemRefNo = refNo;
                        exceptionCode.DeviationStatus = (int)DeviationStatusDTO.Outstanding;
                        exceptionCode.ApprovalStatus = (int)ApprovalStatusDTO.Open;
                        exceptionCode.DateCreated = sibsDate == null ? DateTime.Now : sibsDate;
                        exceptionCode.EntryDate = DateTime.Now;
                        exceptionCode.ApprovalRemarks = remarks == null ? "" : remarks;
                        exceptionCode.IsProcessed = changes == "Create" ? true : false;
                        exceptionCode.Changes = changes == "Update" ? "Create From MERN" : changes;
                        exceptionCode.ModifiedDateTime = changes == "Update" ? DateTime.Now : (DateTime?)null;
                        //exceptionCode.TaggingDate = DateTime.Now;
                        exceptionCodeList.Add(exceptionCode);

                        endValue = subRef.Last();
                    }
                }

                if (changes == "Update")
                {
                    //Delete excluded existing subexception.               
                    var exCodes = _context.ExceptionCodes
                            .Where(s => s.ExItemRefNo == vm.ExceptionItem.RefNo && s.DeviationStatus == (int)DeviationStatusDTO.Outstanding);

                    //var toDelete = exCodes.Where(s => vm.SelectedExCodes.Contains(s.ExCode));
                    //_context.ExceptionCode.RemoveRange(toDelete);

                    List<int> removedExCodes = exCodes.Select(s => s.ExCode).Except(vm.SelectedExCodes).ToList();

                    List<ExceptionCode> toDelete = exCodes.Where(s => removedExCodes.Contains(s.ExCode)).ToList();

                    //Log the deleted sub-exception to revs.
                    foreach (var item in toDelete)
                    {
                        ExceptionCodeRev exceptionCode = new ExceptionCodeRev();
                        exceptionCode.SubReferenceNo = item.SubReferenceNo;
                        exceptionCode.ExCode = item.ExCode;
                        exceptionCode.ExItemRefNo = item.ExItemRefNo;
                        exceptionCode.DeviationStatus = item.DeviationStatus;
                        exceptionCode.ApprovalStatus = item.ApprovalStatus;
                        exceptionCode.DateCreated = item.DateCreated;
                        exceptionCode.EntryDate = (DateTime)item.EntryDate;
                        exceptionCode.ModifiedDateTime = DateTime.Now;
                        exceptionCode.ApprovalRemarks = remarks == null ? "" : remarks;
                        exceptionCode.IsProcessed = false;
                        exceptionCode.Changes = "Delete From MERN";
                        exceptionCodeList.Add(exceptionCode);
                    }
                }

                if (changes == "Delete")
                {
                    var exCodes = _context.ExceptionCodes
                            .Where(s => s.ExItemRefNo == vm.ExceptionItem.RefNo);

                    foreach (var item in exCodes)
                    {
                        var exceptionCode = new ExceptionCodeRev();
                        exceptionCode.SubReferenceNo = item.SubReferenceNo;
                        exceptionCode.ExCode = item.ExCode;
                        exceptionCode.ExItemRefNo = item.ExItemRefNo;
                        exceptionCode.DeviationStatus = item.DeviationStatus;
                        exceptionCode.ApprovalStatus = item.ApprovalStatus;
                        exceptionCode.DateCreated = item.DateCreated;
                        exceptionCode.EntryDate = (DateTime)item.EntryDate;
                        exceptionCode.ApprovalRemarks = item.ApprovalRemarks;
                        exceptionCode.IsProcessed = false;
                        exceptionCode.Changes = "Delete";
                        exceptionCode.ModifiedDateTime = DateTime.Now;
                        //exceptionCode.TaggingDate = DateTime.Now;
                        exceptionCodeList.Add(exceptionCode);
                    }
                }

                #endregion
            }




            if (changes == "Create")
            {
                exceptionItemRevs.Id = Guid.NewGuid();
                exceptionItemRevs.RefNo = refNo;
                exceptionItemRevs.Changes = changes;
                exceptionItemRevs.EmployeeId = vm.EmployeeID;
                exceptionItemRevs.ExceptionCodeRevs = (ICollection<ExceptionCodeRev>)exceptionCodeList;
                exceptionItemRevs.DateCreated = sibsDate == null ? DateTime.Now : sibsDate;
                exceptionItemRevs.EntryDate = DateTime.Now;
                exceptionItemRevs.CreatedBy = userDetail.LoginName;
                exceptionItemRevs.Status = (int)MainStatusDTO.Open;
                exceptionItemRevs.IsProcessed = true;
                if (exceptionItemRevs.Remarks != null)
                {
                    //TempData["Remarks"] = exceptionItemRevs.Remarks;
                }
                exceptionItemRevs.Remarks = "";

                if (exceptionItemRevs.Type == (int)TransactionTypeEnumDTO.Monetary)
                {
                    exceptionItemRevs.MonetaryRevs.ExceptionId = exceptionItemRevs.Id;
                    exceptionItemRevs.MonetaryRevs.RefNo = exceptionItemRevs.RefNo;
                }
                else if (exceptionItemRevs.Type == (int)TransactionTypeEnumDTO.NonMonetary)
                {
                    exceptionItemRevs.NonMonetaryRevs.ExceptionId = exceptionItemRevs.Id;
                    exceptionItemRevs.NonMonetaryRevs.RefNo = exceptionItemRevs.RefNo;
                }
                else
                {
                    exceptionItemRevs.MiscRevs.ExceptionId = exceptionItemRevs.Id;
                    exceptionItemRevs.MiscRevs.RefNo = exceptionItemRevs.RefNo;
                }
                vm.ExceptionItemRevs = exceptionItemRevs;
                _context.Add(vm.ExceptionItemRevs);
            }
            else if (changes == "Update")
            {
                exceptionItemRevs.Id = Guid.NewGuid();
                exceptionItemRevs.RefNo = refNo;
                exceptionItemRevs.Changes = changes;
                exceptionItemRevs.EmployeeId = vm.ExceptionItem.EmployeeId;
                exceptionItemRevs.BranchCode = vm.ExceptionItem.BranchCode;
                exceptionItemRevs.BranchName = vm.ExceptionItem.BranchName;
                exceptionItemRevs.Division = vm.ExceptionItem.Division;
                exceptionItemRevs.Area = vm.ExceptionItem.Area;
                exceptionItemRevs.TransactionDate = vm.ExceptionItem.TransactionDate;
                exceptionItemRevs.DateCreated = vm.ExceptionItem.DateCreated;
                exceptionItemRevs.CreatedBy = vm.ExceptionItem.CreatedBy;
                exceptionItemRevs.Type = vm.ExceptionItem.Type;
                exceptionItemRevs.Severity = vm.ExceptionItem.Severity;
                exceptionItemRevs.Remarks = vm.ExceptionItem.Remarks;
                exceptionItemRevs.RedFlag = vm.ExceptionItem.RedFlag;
                exceptionItemRevs.ModifiedDateTime = DateTime.Now;
                exceptionItemRevs.ModifiedBy = userDetail.LoginName;
                exceptionItemRevs.Status = (int)MainStatusDTO.PendingApproval;
                exceptionItemRevs.IsProcessed = false;
                exceptionItemRevs.DeviationCategoryId = vm.ExceptionItem.DeviationCategoryId;
                exceptionItemRevs.RiskClassificationId = vm.ExceptionItem.RiskClassificationId;
                exceptionItemRevs.RootCause = vm.ExceptionItem.RootCause;
                exceptionItemRevs.DeviationApprover = vm.ExceptionItem.DeviationApprover;
                exceptionItemRevs.AgingCategory = vm.ExceptionItem.AgingCategory;
                exceptionItemRevs.PersonResponsible = vm.ExceptionItem.PersonResponsible;
                exceptionItemRevs.OtherPersonResponsible = vm.ExceptionItem.OtherPersonResponsible;
                exceptionItemRevs.ExceptionCodeRevs = (ICollection<ExceptionCodeRev>)exceptionCodeList;
                exceptionItemRevs.EntryDate = DateTime.Now;
                vm.ExceptionItemRevs = exceptionItemRevs;
                vm.ExceptionItemRevs = SupplyNonRevsTypesFromRevsTypes(vm, (TransactionTypeEnumDTO)vm.ExceptionItem.Type);


                _context.Add(vm.ExceptionItemRevs);
            }
            else if (changes == "Delete")
            {
                //TODO: Deletion Revs For Approval                
                exceptionItemRevs.Id = Guid.NewGuid();
                exceptionItemRevs.RefNo = refNo;
                exceptionItemRevs.Changes = changes;
                exceptionItemRevs.EmployeeId = vm.ExceptionItem.EmployeeId;
                exceptionItemRevs.BranchCode = vm.ExceptionItem.BranchCode;
                exceptionItemRevs.BranchName = vm.ExceptionItem.BranchName;
                exceptionItemRevs.Division = vm.ExceptionItem.Division;
                exceptionItemRevs.Area = vm.ExceptionItem.Area;
                exceptionItemRevs.DeviationApprovedBy = vm.ExceptionItem.DeviationApprovedBy;
                exceptionItemRevs.TransactionDate = vm.ExceptionItem.TransactionDate;
                exceptionItemRevs.DateCreated = vm.ExceptionItem.DateCreated;
                exceptionItemRevs.CreatedBy = vm.ExceptionItem.CreatedBy;
                exceptionItemRevs.Type = vm.ExceptionItem.Type;
                exceptionItemRevs.ModifiedDateTime = DateTime.Now;
                exceptionItemRevs.EntryDate = DateTime.Now;
                exceptionItemRevs.ModifiedBy = userDetail.LoginName;
                exceptionItemRevs.Status = (int)MainStatusDTO.PendingApproval;
                exceptionItemRevs.IsProcessed = false;
                exceptionItemRevs.Remarks = vm.ExceptionItem.Remarks;
                exceptionItemRevs.OtherRemarks = remarks;
                exceptionItemRevs.DeviationCategoryId = vm.ExceptionItem.DeviationCategoryId;
                exceptionItemRevs.RiskClassificationId = vm.ExceptionItem.RiskClassificationId;
                exceptionItemRevs.RootCause = vm.ExceptionItem.RootCause;
                exceptionItemRevs.DeviationApprover = vm.ExceptionItem.DeviationApprover;
                exceptionItemRevs.AgingCategory = vm.ExceptionItem.AgingCategory;
                exceptionItemRevs.PersonResponsible = vm.ExceptionItem.PersonResponsible;
                exceptionItemRevs.OtherPersonResponsible = vm.ExceptionItem.OtherPersonResponsible;
                exceptionItemRevs.ExceptionCodeRevs = exceptionCodeList;
                vm.ExceptionItemRevs = exceptionItemRevs;
                vm.ExceptionItemRevs = SupplyNonRevsTypesFromRevsTypes(vm, (TransactionTypeEnumDTO)vm.ExceptionItem.Type);


                _context.Add(vm.ExceptionItemRevs);
            }
            else if (changes == "Regularize")
            {
                //exceptionItemRevs.Id = Guid.NewGuid();
                //exceptionItemRevs.RefNo = refNo;
                //exceptionItemRevs.Changes = changes;
                //exceptionItemRevs.EmployeeID = vm.ExceptionItem.EmployeeID;
                //exceptionItemRevs.BranchCode = vm.ExceptionItem.BranchCode;
                //exceptionItemRevs.BranchName = vm.ExceptionItem.BranchName;
                //exceptionItemRevs.DeviationApprovedBy = vm.ExceptionItem.DeviationApprovedBy;
                //exceptionItemRevs.TransactionDate = vm.ExceptionItem.TransactionDate;
                //exceptionItemRevs.DateCreated = vm.ExceptionItem.DateCreated;
                //exceptionItemRevs.CreatedBy = vm.ExceptionItem.CreatedBy;
                //exceptionItemRevs.Type = vm.ExceptionItem.Type;
                //exceptionItemRevs.ModifiedDateTime = DateTime.Now;
                //exceptionItemRevs.ModifiedBy = User.FindFirst("UserName").Value;
                //exceptionItemRevs.Status = Status.Regularized;
                //exceptionItemRevs.IsProcessed = true;
                //exceptionItemRevs.Remarks = vm.ExceptionItem.Remarks;
                //exceptionItemRevs.ExCodeRevs = ConvertExCodeToRev(null, vm.ExceptionItem.ExCode, refNo);
                //vm.ExceptionItemRevs = exceptionItemRevs;
                //vm.ExceptionItemRevs = SupplyNonRevsTypesFromRevsTypes(vm, vm.ExceptionItem.Type);

                //_context.Add(vm.ExceptionItemRevs);
            }
        }
        private string GetNextSubSeries(string refNo, char endValue)
        {
            string result = string.Empty;
            char[] alpha = "@ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            ExceptionCode lastSequence = null;

            //var lastSequence = _context.SubRefNoSequence.Where(s => s.ERN == refNo).OrderBy(s => s.Series).LastOrDefault();
            if (endValue == alpha[0])
            {
                lastSequence = _context.ExceptionCodes.Where(s => s.ExItemRefNo == refNo).OrderBy(s => s.SubReferenceNo).LastOrDefault();
            }

            if (lastSequence != null)
            {
                var index = Array.IndexOf(alpha, Convert.ToChar(lastSequence.SubReferenceNo.Substring(19)));
                index++;
                endValue = alpha[index];
            }
            else
            {
                var index = Array.IndexOf(alpha, endValue);
                index++;
                endValue = alpha[index];
            }

            result = $"{refNo}-{endValue}";

            return result;
        }
        private ExceptionItemRev SupplyNonRevsTypesFromRevsTypes(ExceptionViewDTO vm, TransactionTypeEnumDTO type)
        {
            ExceptionItemRevsDTO itemRevs = new ExceptionItemRevsDTO();

            switch (type)
            {
                case TransactionTypeEnumDTO.Monetary:
                    MonetaryRev monetaryRevs = new MonetaryRev()
                    {
                        SequenceNo = vm.ExceptionItem.Monetaries.First().SequenceNo,
                        BdstellerId = vm.ExceptionItem.Monetaries.First().BdstellerId,
                        Amount = vm.ExceptionItem.Monetaries.First().Amount,
                        TransCode = vm.ExceptionItem.Monetaries.First().TransCode,
                        TransDescription = vm.ExceptionItem.Monetaries.First().TransDescription,
                        CreditAccountName = vm.ExceptionItem.Monetaries.First().CreditAccountName,
                        CreditAccountNo = vm.ExceptionItem.Monetaries.First().CreditAccountNo,
                        DebitAccountName = vm.ExceptionItem.Monetaries.First().DebitAccountName,
                        DebitAccountNo = vm.ExceptionItem.Monetaries.First().DebitAccountNo,
                        ExceptionId = vm.ExceptionItem.Monetaries.First().ExceptionId,
                        RefNo = vm.ExceptionItem.Monetaries.First().RefNo,
                        Currency = vm.ExceptionItem.Monetaries.First().Currency

                    };
                    vm.ExceptionItemRevs.MonetaryRevs = monetaryRevs;
                    break;
                case TransactionTypeEnumDTO.NonMonetary:
                    NonMonetaryRev nonMonetaryRevs = new NonMonetaryRev()
                    {
                        //Id = vm.ExceptionItemRevs.NonMonetaryRevs.Id,
                        Type = vm.ExceptionItem.NonMonetaries.First().Category,
                        Cifnumber = vm.ExceptionItem.NonMonetaries.First().Cifnumber,
                        CustomerName = vm.ExceptionItem.NonMonetaries.First().CustomerName,
                        CustomerAccountNo = vm.ExceptionItem.NonMonetaries.First().CustomerAccountNo,
                        ExceptionId = vm.ExceptionItem.NonMonetaries.First().ExceptionId,
                        RefNo = vm.ExceptionItem.NonMonetaries.First().RefNo
                    };
                    vm.ExceptionItemRevs.NonMonetaryRevs = nonMonetaryRevs;
                    break;
                case TransactionTypeEnumDTO.Miscellaneous:
                    MiscRev miscRevs = new MiscRev()
                    {
                        Type = vm.ExceptionItem.Miscs.First().Category,
                        CardNo = vm.ExceptionItem.Miscs.First().CardNo,
                        BankCertNo = vm.ExceptionItem.Miscs.First().BankCertNo,
                        GlslaccountNo = vm.ExceptionItem.Miscs.First().GlslaccountNo,
                        GlslaccountName = vm.ExceptionItem.Miscs.First().GlslaccountName,
                        Dpafno = vm.ExceptionItem.Miscs.First().Dpafno,
                        CheckNo = vm.ExceptionItem.Miscs.First().CheckNo,
                        Amount = vm.ExceptionItem.Miscs.First().Amount,
                        ExceptionId = vm.ExceptionItem.Miscs.First().ExceptionId,
                        RefNo = vm.ExceptionItem.Miscs.First().RefNo
                    };
                    vm.ExceptionItemRevs.MiscRevs = miscRevs;
                    break;
                default:
                    break;
            }

            return vm.ExceptionItemRevs;
        }
        private void SaveToExceptionItem(ExceptionViewDTO vm, TransactionTypeEnumDTO type, string refNo)
        {
            //if (vm.ExceptionItem == null)
            //    return;
            ExceptionItem exceptionItem = new ExceptionItem();
            exceptionItem.Id = Guid.NewGuid();
            exceptionItem.RefNo = vm.ExceptionItemRevs.RefNo;
            exceptionItem.EmployeeId = vm.ExceptionItemRevs.EmployeeId;
            exceptionItem.BranchCode = vm.ExceptionItemRevs.BranchCode;
            exceptionItem.BranchName = vm.ExceptionItemRevs.BranchName;
            exceptionItem.Division = vm.ExceptionItemRevs.Division;
            exceptionItem.Area = vm.ExceptionItemRevs.Area;
            exceptionItem.PersonResponsible = vm.ExceptionItemRevs.PersonResponsible;
            exceptionItem.OtherPersonResponsible = vm.ExceptionItemRevs.OtherPersonResponsible;
            exceptionItem.Severity = (int)vm.ExceptionItemRevs.Severity;
            exceptionItem.DeviationApprovedBy = vm.ExceptionItemRevs.DeviationApprovedBy;
            if (exceptionItem.Remarks != null)
            {
                //exceptionItem.Remarks = TempData["Remarks"].ToString();
            }
            exceptionItem.RedFlag = vm.ExceptionItemRevs.RedFlag;
            exceptionItem.TransactionDate = vm.ExceptionItemRevs.TransactionDate;
            exceptionItem.DateCreated = vm.ExceptionItemRevs.DateCreated;
            exceptionItem.CreatedBy = vm.ExceptionItemRevs.CreatedBy;
            exceptionItem.Status = (int)vm.ExceptionItemRevs.Status;
            exceptionItem.Type = (int)vm.ExceptionItemRevs.Type;
            exceptionItem.DeviationCategoryId = vm.ExceptionItemRevs.DeviationCategoryId;
            exceptionItem.RootCause = (int)vm.ExceptionItemRevs.RootCause;
            exceptionItem.DeviationApprover = vm.ExceptionItemRevs.DeviationApprover;
            exceptionItem.AgingCategory = (int)vm.ExceptionItemRevs.AgingCategory;
            exceptionItem.Age = vm.ExceptionItemRevs.Age;
            exceptionItem.RiskClassificationId = vm.ExceptionItemRevs.RiskClassificationId;
            exceptionItem.EntryDate = vm.ExceptionItemRevs.EntryDate;

            List<ExceptionCode> exceptionCodeList = ConvertExCodeToNonRev(vm.SelectedExCodes, null, refNo);
            exceptionItem.ExceptionCodes = exceptionCodeList;

            switch (type)
            {
                case TransactionTypeEnumDTO.Monetary:
                    Monetary monetary = new Monetary()
                    {
                        //Id = vm.ExceptionItemRevs.MonetaryRevs.Id,
                        SequenceNo = vm.ExceptionItemRevs.MonetaryRevs.SequenceNo,
                        BdstellerId = vm.ExceptionItemRevs.MonetaryRevs.BdstellerId,
                        Amount = vm.ExceptionItemRevs.MonetaryRevs.Amount,
                        TransCode = vm.ExceptionItemRevs.MonetaryRevs.TransCode,
                        TransDescription = vm.ExceptionItemRevs.MonetaryRevs.TransDescription,
                        CreditAccountName = vm.ExceptionItemRevs.MonetaryRevs.CreditAccountName,
                        CreditAccountNo = vm.ExceptionItemRevs.MonetaryRevs.CreditAccountNo,
                        DebitAccountName = vm.ExceptionItemRevs.MonetaryRevs.DebitAccountName,
                        DebitAccountNo = vm.ExceptionItemRevs.MonetaryRevs.DebitAccountNo,
                        ExceptionId = exceptionItem.Id,
                        RefNo = exceptionItem.RefNo,
                        Currency = (int?)vm.ExceptionItemRevs.MonetaryRevs.Currency
                    };
                    exceptionItem.Monetaries.Add(monetary);
                    break;
                case TransactionTypeEnumDTO.NonMonetary:
                    NonMonetary nonMonetary = new NonMonetary()
                    {
                        //Id = vm.ExceptionItemRevs.NonMonetaryRevs.Id,
                        Category = (int?)vm.ExceptionItemRevs.NonMonetaryRevs.Type,
                        Cifnumber = vm.ExceptionItemRevs.NonMonetaryRevs.Cifnumber,
                        CustomerName = vm.ExceptionItemRevs.NonMonetaryRevs.CustomerName,
                        CustomerAccountNo = vm.ExceptionItemRevs.NonMonetaryRevs.CustomerAccountNo,
                        ExceptionId = exceptionItem.Id,
                        RefNo = exceptionItem.RefNo
                    };
                    exceptionItem.NonMonetaries.Add(nonMonetary);
                    break;
                case TransactionTypeEnumDTO.Miscellaneous:
                    Misc misc = new Misc()
                    {
                        //Id = vm.ExceptionItemRevs.MiscRevs.Id,
                        Category = vm.ExceptionItemRevs.MiscRevs.Type,
                        CardNo = vm.ExceptionItemRevs.MiscRevs.CardNo,
                        BankCertNo = vm.ExceptionItemRevs.MiscRevs.BankCertNo,
                        GlslaccountNo = vm.ExceptionItemRevs.MiscRevs.GlslaccountNo,
                        GlslaccountName = vm.ExceptionItemRevs.MiscRevs.GlslaccountName,
                        Dpafno = vm.ExceptionItemRevs.MiscRevs.Dpafno,
                        CheckNo = vm.ExceptionItemRevs.MiscRevs.CheckNo,
                        Amount = vm.ExceptionItemRevs.MiscRevs.Amount,
                        ExceptionId = exceptionItem.Id,
                        RefNo = exceptionItem.RefNo
                    };
                    exceptionItem.Miscs.Add(misc);
                    break;
                default:
                    break;
            }

            _context.Add(exceptionItem);
            _context.SaveChanges();
        }
        private List<ExceptionCode> ConvertExCodeToNonRev(List<int> selectedCodes, ICollection<ExceptionCodeRevsDTO> exCodeRevs, string refNo)
        {
            if (selectedCodes == null)
            {
                selectedCodes = new List<int>();
                foreach (var item in exCodeRevs)
                {
                    selectedCodes.Add(item.ExCode);
                }
            }

            var exceptionCodeList = new List<ExceptionCode>();

            char endValue = '@';

            foreach (var item in selectedCodes)
            {
                string subRefNo = GetNextSubSeries(refNo, endValue);

                var exceptionCode = new ExceptionCode();

                exceptionCode.SubReferenceNo = subRefNo;
                exceptionCode.DeviationStatus = (int)DeviationStatusDTO.Outstanding;
                exceptionCode.ApprovalStatus = (int?)ApprovalStatusDTO.Open;
                exceptionCode.ExCode = item;
                exceptionCode.DateCreated = sibsDate == null ? DateTime.Now : sibsDate;
                exceptionCode.EntryDate = DateTime.Now;
                exceptionCode.ExItemRefNo = refNo;
                exceptionCode.TaggingDate = DateTime.Now;
                exceptionCodeList.Add(exceptionCode);

                endValue = subRefNo.Last();
            }

            return exceptionCodeList;
        }
        private List<string> GetRecipients(ExceptionItem query)
        {
            List<string> recipients = new List<string>();
            List<string> employeesIDs = new List<string>();
            List<int> branchIDs = new List<int>();

            string createdBy = query.CreatedBy;

            var assignedBranches = _context.Users.Include(s => s.BranchAccesses).SingleOrDefault(s => s.LoginName == createdBy).BranchAccesses;

            foreach (var item in assignedBranches)
            {
                employeesIDs.Add(item.EmployeeId);
                branchIDs.Add(item.BranchId);
            }

            var matchingIDs = _context.BranchAccesses.Where(s => branchIDs.Contains(s.BranchId)).Select(s => s.EmployeeId);

            var matchingPeople = _context.Users.Where(x => matchingIDs.Contains(x.EmployeeId));

            if (matchingPeople != null)
            {
                if (matchingPeople.Count() != 0)
                {
                    recipients = matchingPeople.Where(s => s.RoleId == 3 || s.RoleId == 5).Select(s => s.Email).ToList();
                }
            }

            return recipients;
        }

        private void UpdateExceptionItemRevs(ExceptionViewDTO vm, TransactionTypeEnum type, string refNo, string actionTaken)
        {
            vm.ExceptionItemRevs.ApprovedBy = UserLoginName;
            vm.ExceptionItemRevs.ApprovedDateTime = DateTime.Now;
            vm.ExceptionItemRevs.ActionTaken = actionTaken;
            vm.ExceptionItemRevs.IsProcessed = true;
            //vm.ExceptionItemRevs.Remarks = vm.ExceptionItem.Remarks;
            vm.ExceptionItemRevs.ApprovalRemarks = vm.ApprovalRemarks;
            vm.ExceptionItemRevs.Status = (int)MainStatus.Closed;

            //Update ExceptionCodeRevs..
            List<ExceptionCodeRev> toUpdate = new List<ExceptionCodeRev>();

            foreach (var item in vm.ExceptionItemRevs.ExceptionCodeRevs)
            {
                item.ApprovedBy = UserLoginName;
                item.ApprovedDateTime = DateTime.Now;
                item.ActionTaken = actionTaken;
                item.IsProcessed = true;
                item.ApprovalRemarks = string.Empty;
                item.ApprovalStatus = (int?)ApprovalStatusDTO.Closed;
                item.ActionTaken = "Approved";
                toUpdate.Add(item);
            }

            vm.ExceptionItemRevs.ExceptionCodeRevs = toUpdate;
            _context.Update(vm.ExceptionItemRevs);


            if (vm.ExceptionItemRevs.Changes == "Update")
            {
                if (actionTaken == "Approved")
                {
                    ////Create actions for (main)excode table
                    //Remove
                    var toDelete = toUpdate.Where(s => s.Changes == "Delete From MERN").Select(s => s.SubReferenceNo);

                    var test = _context.ExceptionCodes.Where(s => toDelete.Contains(s.SubReferenceNo));
                    _context.RemoveRange(test);

                    //Add
                    List<ExceptionCode> exCodeList = new List<ExceptionCode>();
                    var toCreate = toUpdate.Where(s => s.Changes == "Create From MERN");

                    foreach (var item in toCreate)
                    {
                        ExceptionCode exceptionCode = new ExceptionCode();
                        exceptionCode.SubReferenceNo = item.SubReferenceNo;
                        exceptionCode.ExCode = item.ExCode;
                        exceptionCode.ExItemRefNo = item.ExItemRefNo;
                        exceptionCode.DeviationStatus = (int)DeviationStatus.Outstanding;
                        exceptionCode.ApprovalStatus = (int)ApprovalStatusDTO.Open;
                        exceptionCode.DateCreated = sibsDate == null ? DateTime.Now : sibsDate;
                        exceptionCode.EntryDate = DateTime.Now;
                        exceptionCode.ApprovalRemarks = "";
                        exceptionCode.ExCodeDescription = item.ExCodeDescription;

                        exCodeList.Add(exceptionCode);
                    }

                    if (exCodeList.Count != 0)
                    {
                        _context.AddRange(exCodeList);
                    }
                }
            }
        }
        private void UpdateExceptionItem(ExceptionViewDTO vm, string action)
        {
            ExceptionItem exItem = _context.ExceptionItems.Include(s => s.Monetaries)
                .Include(s => s.NonMonetaries).Include(s => s.Miscs).Include(s => s.ExceptionCodes)
                .SingleOrDefault(s => s.RefNo == vm.ExceptionItemRevs.RefNo);

            if (exItem != null)
            {
                if (action == "Approved")
                {
                    exItem.DeviationApprovedBy = UserLoginName;
                    exItem.Status = (int)MainStatus.Open;
                    exItem.BranchCode = vm.ExceptionItemRevs.BranchCode;
                    exItem.BranchName = vm.ExceptionItemRevs.BranchName;
                    exItem.CreatedBy = vm.ExceptionItemRevs.CreatedBy;
                    exItem.DateCreated = vm.ExceptionItemRevs.DateCreated;
                    exItem.EmployeeId = vm.ExceptionItemRevs.EmployeeId;
                    exItem.RedFlag = vm.ExceptionItemRevs.RedFlag;
                    exItem.RefNo = vm.ExceptionItemRevs.RefNo;
                    exItem.Remarks = vm.ExceptionItemRevs.Remarks;
                    exItem.Severity = vm.ExceptionItemRevs.Severity;
                    exItem.Type = vm.ExceptionItemRevs.Type;
                    exItem.DeviationCategoryId = vm.ExceptionItemRevs.DeviationCategoryId;
                    exItem.RootCause = vm.ExceptionItemRevs.RootCause;
                    exItem.DeviationApprover = vm.ExceptionItemRevs.DeviationApprover == null ? string.Empty : vm.ExceptionItemRevs.DeviationApprover;
                    exItem.AgingCategory = vm.ExceptionItemRevs.AgingCategory;
                    exItem.PersonResponsible = vm.ExceptionItemRevs.PersonResponsible;
                    exItem.OtherPersonResponsible = vm.ExceptionItemRevs.OtherPersonResponsible;
                    //exItem.ExCode = ConvertExCodeToNonRev(null, vm.ExceptionItemRevs.ExCodeRevs, vm.ExceptionItemRevs.RefNo);
                    exItem.ApprovalRemarks = vm.ApprovalRemarks;
                    vm.ExceptionItem = exItem;
                    vm.ExceptionItem = SupplyRevsTypesFromNonRevsTypes(vm, (TransactionTypeEnum)vm.ExceptionItemRevs.Type);
                }
                else
                {
                    exItem.Status = (int)MainStatus.Open;
                    exItem.Remarks = vm.ExceptionItemRevs.Remarks;
                    exItem.ApprovalRemarks = vm.ApprovalRemarks;
                    vm.ExceptionItem = exItem;
                }
            }
            _context.Update(vm.ExceptionItem);
            _context.RemoveRange();
        }
        private void DeleteExceptionItem(ExceptionViewDTO vm, TransactionTypeEnum type, string refNo)
        {
            var exItem = _context.ExceptionItems.Include(s => s.ExceptionCodes).Include(s => s.Monetaries)
                .Include(s => s.NonMonetaries).Include(s => s.Miscs).SingleOrDefault(s => s.RefNo == refNo);



            if (exItem != null)
            {
                vm.ExceptionItem = exItem;
                _context.Remove(vm.ExceptionItem);
            }
        }
        private ExceptionItem SupplyRevsTypesFromNonRevsTypes(ExceptionViewDTO vm, TransactionTypeEnum type)
        {
            ExceptionItem itemRevs = new ExceptionItem();

            switch (type)
            {
                case TransactionTypeEnum.Monetary:
                    Monetary monetary = new Monetary()
                    {
                        SequenceNo = vm.ExceptionItemRevs.MonetaryRevs.SequenceNo,
                        BdstellerId = vm.ExceptionItemRevs.MonetaryRevs.BdstellerId,
                        Amount = vm.ExceptionItemRevs.MonetaryRevs.Amount,
                        TransCode = vm.ExceptionItemRevs.MonetaryRevs.TransCode,
                        TransDescription = vm.ExceptionItemRevs.MonetaryRevs.TransDescription,
                        CreditAccountName = vm.ExceptionItemRevs.MonetaryRevs.CreditAccountName,
                        CreditAccountNo = vm.ExceptionItemRevs.MonetaryRevs.CreditAccountNo,
                        DebitAccountName = vm.ExceptionItemRevs.MonetaryRevs.DebitAccountName,
                        DebitAccountNo = vm.ExceptionItemRevs.MonetaryRevs.DebitAccountNo,
                        ExceptionId = vm.ExceptionItemRevs.MonetaryRevs.ExceptionId,
                        RefNo = vm.ExceptionItemRevs.MonetaryRevs.RefNo,
                        Currency = vm.ExceptionItemRevs.MonetaryRevs.Currency
                    };
                    vm.ExceptionItem.Monetaries.Add(monetary);
                    break;
                case TransactionTypeEnum.NonMonetary:
                    NonMonetary nonMonetary = new NonMonetary()
                    {
                        Category = vm.ExceptionItemRevs.NonMonetaryRevs.Type,
                        Cifnumber = vm.ExceptionItemRevs.NonMonetaryRevs.Cifnumber,
                        CustomerName = vm.ExceptionItemRevs.NonMonetaryRevs.CustomerName,
                        CustomerAccountNo = vm.ExceptionItemRevs.NonMonetaryRevs.CustomerAccountNo,
                        ExceptionId = vm.ExceptionItemRevs.NonMonetaryRevs.ExceptionId,
                        RefNo = vm.ExceptionItemRevs.NonMonetaryRevs.RefNo
                    };
                    vm.ExceptionItem.NonMonetaries.Add(nonMonetary);
                    break;
                case TransactionTypeEnum.Miscellaneous:
                    Misc misc = new Misc()
                    {
                        Category = vm.ExceptionItemRevs.MiscRevs.Type,
                        CardNo = vm.ExceptionItemRevs.MiscRevs.CardNo,
                        BankCertNo = vm.ExceptionItemRevs.MiscRevs.BankCertNo,
                        GlslaccountNo = vm.ExceptionItemRevs.MiscRevs.GlslaccountNo,
                        GlslaccountName = vm.ExceptionItemRevs.MiscRevs.GlslaccountName,
                        Dpafno = vm.ExceptionItemRevs.MiscRevs.Dpafno,
                        CheckNo = vm.ExceptionItemRevs.MiscRevs.CheckNo,
                        Amount = vm.ExceptionItemRevs.MiscRevs.Amount,
                        ExceptionId = vm.ExceptionItemRevs.MiscRevs.ExceptionId,
                        RefNo = vm.ExceptionItemRevs.MiscRevs.RefNo
                    };
                    vm.ExceptionItem.Miscs.Add(misc);
                    break;
                default:
                    break;
            }

            return vm.ExceptionItem;
        }

        #endregion
    }
}
