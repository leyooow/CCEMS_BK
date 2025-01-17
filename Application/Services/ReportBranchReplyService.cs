using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Report;
using Application.Models.Helpers;
using Application.Models.Responses;
using Application.Services.Application.Services;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReportBranchReplyService : IReportBranchReplyService
    {
        private readonly IReportBranchReplyRepository _repository;
        private readonly UserClaimsDTO _user;
        private readonly IUserClaimsService _userClaimsService;
        public ReportBranchReplyService(IReportBranchReplyRepository repository, IUserClaimsService userClaimsService) 
        { 
            _repository = repository;
            _userClaimsService = userClaimsService;
            _user = _userClaimsService.GetClaims();
        }
        public async Task<PaginatedList<ReportContent>> GetReportContentList(string id, string refNo, int page = 1)
        {
            return await _repository.GetReportContentList(id, refNo, page);
        }
        public async Task<List<BranchReply>> GetBranchReplyList(string id)
        {
            var x = await _repository.GetBranchReplyList(id);
            if(x.Count > 0)
                x[0].IsHighlight = true;
            return x;
        }
        public async Task<GenericResponse<dynamic>> PostReply(BranchReplyViewModel vm)
        {
            string loggedUser = _user.LoginName ?? string.Empty;
            if (vm.ActionPlan != null)
            {
                if (vm.ActionPlan != string.Empty)
                {
                    try
                    {
                        ReportContent rContent = new ReportContent();
                        rContent = await _repository.GetReportContents(Guid.Parse(vm.ReportContentsId));
                        rContent.ActionPlan = vm.ActionPlan;

                        BranchReply bReply = new BranchReply
                        {
                            Id = Guid.NewGuid(),
                            ActionPlan = vm.ActionPlan,
                            CreatedBy = loggedUser,
                            DateCreated = DateTime.Now,
                            ReportContentsId = Guid.Parse(vm.ReportContentsId),
                            ExceptionNo = rContent.ExceptionNo
                        };


                        await _repository.SavePostReply(rContent, bReply);
                        return ResponseHelper.SuccessResponse<dynamic>("Report Action Plan Updated!");

                    }
                    catch (Exception ex)
                    {
                        return ResponseHelper.ErrorResponse<dynamic>(ex.Message);
                    }
                }
                else
                {
                    return ResponseHelper.ErrorResponse<dynamic>("Action Plan field is blank!");
                }
            }
            else
            {
                return ResponseHelper.ErrorResponse<dynamic>("Action Plan field is blank!");
            }
        }
    }
}
