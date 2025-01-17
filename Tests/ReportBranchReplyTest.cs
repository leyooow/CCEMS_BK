using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Report;
using Application.Models.Helpers;
using Application.Services;
using AutoMapper;
using Infrastructure.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class ReportBranchReplyTest
    {
        private Mock<IReportBranchReplyRepository> _repo;
        private Mock<IUserClaimsService> _userClaims;
        //private readonly IMapper _mapper;

        public ReportBranchReplyTest()
        {
            _repo = new Mock<IReportBranchReplyRepository>();
            _userClaims = new Mock<IUserClaimsService>();

            //var configuration = new MapperConfiguration(cfg =>
            //{
            //    // Add your AutoMapper configurations here if needed.
            //    cfg.CreateMap<Report, ReportDTO>();
            //});

            //_mapper = configuration.CreateMapper();
        }

        public ReportBranchReplyService Subject()
        {
            var mockUserClaims = new UserClaimsDTO
            {
                EmployeeID = "E12345",
                RoleID = 1,
                RoleName = "BOCCH",
                Name = "John Doe",
                LoginName = "jdoe",
                LoginDateTime = "2025-01-06 09:30:00"
            };
            _userClaims.Setup(service => service.GetClaims()).Returns(mockUserClaims);
            return new ReportBranchReplyService(_repo.Object, _userClaims.Object);
        }
        [Fact]
        public async Task GetReportContentList()
        {
            var service = Subject();
            var reportContents = new List<ReportContent>
            {
                new ReportContent
                {
                    Id = Guid.NewGuid(),
                    ReportId = 1,
                    ExceptionNo = "EX001",
                    BranchCode = "BC001",
                    BranchName = "Main Branch",
                    Area = "North",
                    Division = "Division A",
                    TransactionDate = "2025-01-01",
                    Aging = "30",
                    AgingCategory = "1-30 Days",
                    Process = "Loan Processing",
                    AccountNo = "1234567890",
                    AccountName = "John Doe",
                    Deviation = "High",
                    RiskClassification = "Critical",
                    DeviationCategory = "Fraud",
                    Amount = 10000.50M,
                    PersonResponsible = "Manager A",
                    OtherPersonResponsible = "Assistant B",
                    Remarks = "Urgent attention needed",
                    ActionPlan = "Review immediately",
                    EncodedBy = "UserX",
                    RootCause = "System Error",
                    DeviationApprover = "Supervisor C"
                },
                new ReportContent
                {
                    Id = Guid.NewGuid(),
                    ReportId = 2,
                    ExceptionNo = "EX002",
                    BranchCode = "BC002",
                    BranchName = "Second Branch",
                    Area = "South",
                    Division = "Division B",
                    TransactionDate = "2025-01-02",
                    Aging = "45",
                    AgingCategory = "31-60 Days",
                    Process = "Account Review",
                    AccountNo = "9876543210",
                    AccountName = "Jane Smith",
                    Deviation = "Moderate",
                    RiskClassification = "High",
                    DeviationCategory = "Compliance",
                    Amount = 2000.75M,
                    PersonResponsible = "Manager B",
                    OtherPersonResponsible = "Assistant C",
                    Remarks = "Follow-up required",
                    ActionPlan = "Schedule meeting",
                    EncodedBy = "UserY",
                    RootCause = "Human Error",
                    DeviationApprover = "Supervisor D"
                }
            };
            PaginatedList<ReportContent> result = PaginatedList<ReportContent>.Create(reportContents, 1);

            _repo.Setup(r => r.GetReportContentList(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(result);

            var data = await service.GetReportContentList("","",1);

            Assert.NotNull(data);
        }
        [Fact]
        public async Task GetBranchReplyList()
        {
            var service = Subject(); 
            var result = new List<BranchReply>
            {
                new BranchReply
                {
                    Id = Guid.NewGuid(),
                    ReportContentsId = Guid.NewGuid(),
                    CreatedBy = "User1",
                    ActionPlan = "Review and approve the report",
                    DateCreated = DateTime.Now.AddDays(-2),
                    ExceptionNo = "EX001",
                    IsHighlight = false
                },
                new BranchReply
                {
                    Id = Guid.NewGuid(),
                    ReportContentsId = Guid.NewGuid(),
                    CreatedBy = "User2",
                    ActionPlan = "Verify branch compliance",
                    DateCreated = DateTime.Now.AddDays(-1),
                    ExceptionNo = "EX002",
                    IsHighlight = false
                },
                new BranchReply
                {
                    Id = Guid.NewGuid(),
                    ReportContentsId = Guid.NewGuid(),
                    CreatedBy = "User3",
                    ActionPlan = "Update the action plan by end of week",
                    DateCreated = DateTime.Now,
                    ExceptionNo = "EX003",
                    IsHighlight = false
                }
            };

            _repo.Setup(r => r.GetBranchReplyList(It.IsAny<string>())).ReturnsAsync(result);

            var data = await service.GetBranchReplyList("");

            Assert.NotNull(data);
            Assert.Equal(true, data[0].IsHighlight);
        }
        [Fact]
        public async Task PostReply_ShouldReturnSuccessResponse_WhenActionPlanIsValid()
        {
            // Arrange
            var service = Subject();
            var mockReportContentId = Guid.NewGuid().ToString();
            var mockReportContent = new ReportContent
            {
                Id = Guid.Parse(mockReportContentId),
                ExceptionNo = "EX001",
                ActionPlan = null
            };

            var branchReplyViewModel = new BranchReplyViewModel
            {
                ActionPlan = "Implement new guidelines",
                ReportContentsId = mockReportContentId
            };

            _repo.Setup(r => r.GetReportContents(Guid.Parse(mockReportContentId)))
                 .ReturnsAsync(mockReportContent);

            _repo.Setup(r => r.SavePostReply(It.IsAny<ReportContent>(), It.IsAny<BranchReply>()))
                 .Returns(Task.CompletedTask);

            // Act
            var response = await service.PostReply(branchReplyViewModel);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal("Report Action Plan Updated!", response.Message);
            _repo.Verify(r => r.GetReportContents(Guid.Parse(mockReportContentId)), Times.Once);
            _repo.Verify(r => r.SavePostReply(It.IsAny<ReportContent>(), It.IsAny<BranchReply>()), Times.Once);
        }
        [Fact]
        public async Task PostReply_ShouldReturnErrorResponse_WhenActionPlanIsBlank()
        {
            // Arrange
            var service = Subject();
            var branchReplyViewModel = new BranchReplyViewModel
            {
                ActionPlan = string.Empty,
                ReportContentsId = Guid.NewGuid().ToString()
            };

            // Act
            var response = await service.PostReply(branchReplyViewModel);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal("Action Plan field is blank!", response.Message);
            _repo.Verify(r => r.GetReportContents(It.IsAny<Guid>()), Times.Never);
            _repo.Verify(r => r.SavePostReply(It.IsAny<ReportContent>(), It.IsAny<BranchReply>()), Times.Never);
        }
        [Fact]
        public async Task PostReply_ShouldReturnErrorResponse_WhenActionPlanIsNull()
        {
            // Arrange
            var service = Subject();
            var branchReplyViewModel = new BranchReplyViewModel
            {
                ActionPlan = null,
                ReportContentsId = Guid.NewGuid().ToString()
            };

            // Act
            var response = await service.PostReply(branchReplyViewModel);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal("Action Plan field is blank!", response.Message);
            _repo.Verify(r => r.GetReportContents(It.IsAny<Guid>()), Times.Never);
            _repo.Verify(r => r.SavePostReply(It.IsAny<ReportContent>(), It.IsAny<BranchReply>()), Times.Never);
        }
        [Fact]
        public async Task PostReply_ShouldReturnErrorResponse_WhenExceptionOccurs()
        {
            // Arrange
            var service = Subject();
            var mockReportContentId = Guid.NewGuid().ToString();
            var branchReplyViewModel = new BranchReplyViewModel
            {
                ActionPlan = "Implement new guidelines",
                ReportContentsId = mockReportContentId
            };

            _repo.Setup(r => r.GetReportContents(Guid.Parse(mockReportContentId)))
                 .ThrowsAsync(new Exception("Database error"));

            // Act
            var response = await service.PostReply(branchReplyViewModel);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal("Database error", response.Message);
            _repo.Verify(r => r.GetReportContents(Guid.Parse(mockReportContentId)), Times.Once);
            _repo.Verify(r => r.SavePostReply(It.IsAny<ReportContent>(), It.IsAny<BranchReply>()), Times.Never);
        }

    }
}
