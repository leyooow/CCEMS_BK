using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Report;
using Application.Models.DTOs.User.user;
using Application.Models.Helpers;
using Application.Models.Responses;
using Application.Services.Application.Services;
using AutoMapper;
using Infrastructure.Entities;

namespace Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;
        private readonly IUserClaimsService _userClaimsService;
        private readonly UserClaimsDTO _user;
        private readonly IMapper _mapper;
        public ReportService(IReportRepository repository, IUserClaimsService userClaimsService, IMapper mapper) 
        { 
            _repository = repository;
            _userClaimsService = userClaimsService;
            _user = _userClaimsService.GetClaims();
            _mapper = mapper;

        }
        public async Task<PaginatedList<ReportDTO>> GetList(string searchString = "", int Page = 1)
        {
            string loggedRole = _user.RoleName ?? "";
            string empID = _user.EmployeeID ?? string.Empty;
            List<string> employeesAccess = await _repository.GetEmeployeesWithAccess(empID);
            IQueryable<Report> query = Enumerable.Empty<Report>().AsQueryable();
            switch (loggedRole)
            {
                case "BCA":
                case "BCO":
                case "Sales DH":
                    query = _repository.GetList(searchString,employeesAccess);
                    break;

                case "AOO":
                    query = _repository.GetListFilterWithPendingAndApproved(searchString, employeesAccess);
                    break;

                case "BOO":
                case "BM":
                case "BMO":
                case "BOCCH":
                    query = _repository.GetListFilterWithApproved(searchString, employeesAccess);
                    break;

                default:
                    query = _repository.GetEmptyList();
                    break;
            }

            PaginatedList<Report, ReportDTO> data  = await PaginatedList<Report, ReportDTO>.CreateAsync(query, _mapper, Page);
            return new PaginatedList<ReportDTO>(data.Data, data.CountData, data.PageIndex);

        }
        public async Task<List<Group>> PopulateGroupsDropDownList()
        {
            string empID = _user.EmployeeID ?? "";
            return await _repository.PopulateGroupsDropDownList(empID);
        }
        public async Task<GenericResponse<EPPlusReturn>> DownloadAdhoc(DownloadAdhocViewModel vm)
        {
            try
            {
                string empID = _user.EmployeeID ?? string.Empty;
                switch (vm.ReportAdhoc)
                {
                    case ReportAdhoc.Pervasiveness:
                        return await _repository.GeneratePervasivenessLogic(vm);
                    case ReportAdhoc.RegularizationTAT:
                        return await _repository.GenerateRegularizationTATLogic(vm);
                    case ReportAdhoc.AuditTrail:
                        break;
                    case ReportAdhoc.ExceptionAdhocs:
                        return await _repository.GenerateExceptionAdhocsLogic(vm, empID);
                    default:
                        break;
                }
                return ResponseHelper.ErrorResponse<EPPlusReturn>("No extracted data from the filtered query");
            }
            catch (Exception ex)
            {
                return ResponseHelper.ErrorResponse<EPPlusReturn>(ex.Message);
            }
        }
    }
}
