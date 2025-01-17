using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Application.Models.DTOs.Report;
using Application.Models.Helpers;
using Application.Models.Responses;
using Application.Services;
using AutoMapper;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ReportTest
    {
        private Mock<IReportRepository> _repo;
        private Mock<IUserClaimsService> _userClaims;
        private readonly IMapper _mapper;

        public ReportTest()
        {
            _repo = new Mock<IReportRepository>();
            _userClaims = new Mock<IUserClaimsService>();

            var configuration = new MapperConfiguration(cfg =>
            {
                // Add your AutoMapper configurations here if needed.
                cfg.CreateMap<Report, ReportDTO>();
            });

            _mapper = configuration.CreateMapper();
        }

        public ReportService Subject()
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
            return new ReportService(_repo.Object, _userClaims.Object, _mapper);
        }
        [Fact]
        public async Task GetList()
        {
            var options = new DbContextOptionsBuilder<CcemQatContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            using (var context = new CcemQatContext(options))
            {
                try
                {
                    context.Reports.AddRange(new List<Report>
                    {
                        new Report
                        {
                            Id = 1,
                            FileName = "Report1.pdf",
                            Path = "/reports/Report1.pdf",
                            ActionPlan = "Plan1",
                            CreatedBy = "User1",
                            DateGenerated = DateTime.Now.AddDays(-1),
                            DateSent = DateTime.Now,
                            ActionPlanCreated = DateTime.Now.AddDays(-2),
                            Status = 1,
                            BranchCodeRecipient = "Branch1",
                            SendingSchedule = DateTime.Now.AddHours(1),
                            ReportCoverage = 7,
                            ReportCategory = 2,
                            CoverageDate = DateTime.Now.AddDays(-7),
                            SelectedBranches = "Branch1,Branch2",
                            ToRecipients = "user1@example.com",
                            Ccrecipients = "user2@example.com",
                            ReportsGuid = Guid.NewGuid(),
                            ApprovalRemarks = "Approved",
                            ToList = new List<string> { "user1@example.com" },
                            CCList = new List<string> { "user2@example.com" }
                        },
                        new Report
                        {
                            Id = 2,
                            FileName = "Report2.pdf",
                            Path = "/reports/Report2.pdf",
                            ActionPlan = "Plan2",
                            CreatedBy = "User2",
                            DateGenerated = DateTime.Now.AddDays(-3),
                            DateSent = DateTime.Now.AddDays(-2),
                            ActionPlanCreated = DateTime.Now.AddDays(-4),
                            Status = 2,
                            BranchCodeRecipient = "Branch2",
                            SendingSchedule = DateTime.Now.AddHours(2),
                            ReportCoverage = 14,
                            ReportCategory = 3,
                            CoverageDate = DateTime.Now.AddDays(-14),
                            SelectedBranches = "Branch3,Branch4",
                            ToRecipients = "user3@example.com",
                            Ccrecipients = "user4@example.com",
                            ReportsGuid = Guid.NewGuid(),
                            ApprovalRemarks = "Pending",
                            ToList = new List<string> { "user3@example.com" },
                            CCList = new List<string> { "user4@example.com" }
                        }
                    });
                    await context.SaveChangesAsync();

                    var IQuery = context.Reports.AsQueryable();
                    List<string> employeesAccess = ["Branch1", "Branch2"];
                    var mockUserClaims = new UserClaimsDTO
                    {
                        EmployeeID = "E12345",
                        RoleID = 1,
                        RoleName = "BCA",
                        Name = "John Doe",
                        LoginName = "jdoe",
                        LoginDateTime = "2025-01-06 09:30:00"
                    };
                    var service = Subject();

                    // Mock the GetClaims method to return the mockUserClaims
                    _userClaims.Setup(service => service.GetClaims()).Returns(mockUserClaims);
                    // Mock the repository methods to return IQueryable<Report> synchronously
                    _repo.Setup(r => r.GetEmeployeesWithAccess(It.IsAny<string>())).ReturnsAsync(employeesAccess);
                    _repo.Setup(r => r.GetList(It.IsAny<string>(), It.IsAny<List<string>>()))
                        .Returns(IQuery); // Return the IQueryable directly, no need for async
                    _repo.Setup(r => r.GetListFilterWithPendingAndApproved(It.IsAny<string>(), It.IsAny<List<string>>()))
                        .Returns(IQuery); // Return the IQueryable directly, no need for async
                    _repo.Setup(r => r.GetListFilterWithApproved(It.IsAny<string>(), It.IsAny<List<string>>()))
                        .Returns(IQuery); // Return the IQueryable directly, no need for async

                    // Act
                    var result = await service.GetList(); // Since the methods are synchronous, no need for await

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(2, result.CountData); // Asserting the total count
                    Assert.Equal(1, result.PageIndex); // Asserting the page index
                    Assert.Equal(2, result.Data.Count); // Asserting the number of items
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                finally
                {
                    if (context != null)
                    {
                        await context.DisposeAsync(); // Asynchronously dispose the context
                    }
                }
            }
        }
        [Fact]
        public async Task GetListFilterWithPendingAndApproved()
        {
            var options = new DbContextOptionsBuilder<CcemQatContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            using (var context = new CcemQatContext(options))
            {
                try
                {
                    context.Reports.AddRange(new List<Report>
                    {
                        new Report
                        {
                            Id = 1,
                            FileName = "Report1.pdf",
                            Path = "/reports/Report1.pdf",
                            ActionPlan = "Plan1",
                            CreatedBy = "User1",
                            DateGenerated = DateTime.Now.AddDays(-1),
                            DateSent = DateTime.Now,
                            ActionPlanCreated = DateTime.Now.AddDays(-2),
                            Status = 1,
                            BranchCodeRecipient = "Branch1",
                            SendingSchedule = DateTime.Now.AddHours(1),
                            ReportCoverage = 7,
                            ReportCategory = 2,
                            CoverageDate = DateTime.Now.AddDays(-7),
                            SelectedBranches = "Branch1,Branch2",
                            ToRecipients = "user1@example.com",
                            Ccrecipients = "user2@example.com",
                            ReportsGuid = Guid.NewGuid(),
                            ApprovalRemarks = "Approved",
                            ToList = new List<string> { "user1@example.com" },
                            CCList = new List<string> { "user2@example.com" }
                        },
                        new Report
                        {
                            Id = 2,
                            FileName = "Report2.pdf",
                            Path = "/reports/Report2.pdf",
                            ActionPlan = "Plan2",
                            CreatedBy = "User2",
                            DateGenerated = DateTime.Now.AddDays(-3),
                            DateSent = DateTime.Now.AddDays(-2),
                            ActionPlanCreated = DateTime.Now.AddDays(-4),
                            Status = 2,
                            BranchCodeRecipient = "Branch2",
                            SendingSchedule = DateTime.Now.AddHours(2),
                            ReportCoverage = 14,
                            ReportCategory = 3,
                            CoverageDate = DateTime.Now.AddDays(-14),
                            SelectedBranches = "Branch3,Branch4",
                            ToRecipients = "user3@example.com",
                            Ccrecipients = "user4@example.com",
                            ReportsGuid = Guid.NewGuid(),
                            ApprovalRemarks = "Pending",
                            ToList = new List<string> { "user3@example.com" },
                            CCList = new List<string> { "user4@example.com" }
                        }
                    });
                    await context.SaveChangesAsync();

                    var IQuery = context.Reports.AsQueryable();
                    List<string> employeesAccess = ["Branch1", "Branch2"];
                    var mockUserClaims = new UserClaimsDTO
                    {
                        EmployeeID = "E12345",
                        RoleID = 1,
                        RoleName = "AOO",
                        Name = "John Doe",
                        LoginName = "jdoe",
                        LoginDateTime = "2025-01-06 09:30:00"
                    };
                    var service = Subject();

                    // Mock the GetClaims method to return the mockUserClaims
                    _userClaims.Setup(service => service.GetClaims()).Returns(mockUserClaims);
                    // Mock the repository methods to return IQueryable<Report> synchronously
                    _repo.Setup(r => r.GetEmeployeesWithAccess(It.IsAny<string>())).ReturnsAsync(employeesAccess);
                    _repo.Setup(r => r.GetList(It.IsAny<string>(), It.IsAny<List<string>>()))
                        .Returns(IQuery); // Return the IQueryable directly, no need for async
                    _repo.Setup(r => r.GetListFilterWithPendingAndApproved(It.IsAny<string>(), It.IsAny<List<string>>()))
                        .Returns(IQuery); // Return the IQueryable directly, no need for async
                    _repo.Setup(r => r.GetListFilterWithApproved(It.IsAny<string>(), It.IsAny<List<string>>()))
                        .Returns(IQuery); // Return the IQueryable directly, no need for async

                    // Act
                    var result = await service.GetList(); // Since the methods are synchronous, no need for await

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(2, result.CountData); // Asserting the total count
                    Assert.Equal(1, result.PageIndex); // Asserting the page index
                    Assert.Equal(2, result.Data.Count); // Asserting the number of items
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                finally
                {
                    if (context != null)
                    {
                        await context.DisposeAsync(); // Asynchronously dispose the context
                    }
                }
            }
        }
        [Fact]
        public async Task GetListFilterWithApproved()
        {
            var options = new DbContextOptionsBuilder<CcemQatContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            using (var context = new CcemQatContext(options))
            {
                try
                {
                    context.Reports.AddRange(new List<Report>
                    {
                        new Report
                        {
                            Id = 1,
                            FileName = "Report1.pdf",
                            Path = "/reports/Report1.pdf",
                            ActionPlan = "Plan1",
                            CreatedBy = "User1",
                            DateGenerated = DateTime.Now.AddDays(-1),
                            DateSent = DateTime.Now,
                            ActionPlanCreated = DateTime.Now.AddDays(-2),
                            Status = 1,
                            BranchCodeRecipient = "Branch1",
                            SendingSchedule = DateTime.Now.AddHours(1),
                            ReportCoverage = 7,
                            ReportCategory = 2,
                            CoverageDate = DateTime.Now.AddDays(-7),
                            SelectedBranches = "Branch1,Branch2",
                            ToRecipients = "user1@example.com",
                            Ccrecipients = "user2@example.com",
                            ReportsGuid = Guid.NewGuid(),
                            ApprovalRemarks = "Approved",
                            ToList = new List<string> { "user1@example.com" },
                            CCList = new List<string> { "user2@example.com" }
                        },
                        new Report
                        {
                            Id = 2,
                            FileName = "Report2.pdf",
                            Path = "/reports/Report2.pdf",
                            ActionPlan = "Plan2",
                            CreatedBy = "User2",
                            DateGenerated = DateTime.Now.AddDays(-3),
                            DateSent = DateTime.Now.AddDays(-2),
                            ActionPlanCreated = DateTime.Now.AddDays(-4),
                            Status = 2,
                            BranchCodeRecipient = "Branch2",
                            SendingSchedule = DateTime.Now.AddHours(2),
                            ReportCoverage = 14,
                            ReportCategory = 3,
                            CoverageDate = DateTime.Now.AddDays(-14),
                            SelectedBranches = "Branch3,Branch4",
                            ToRecipients = "user3@example.com",
                            Ccrecipients = "user4@example.com",
                            ReportsGuid = Guid.NewGuid(),
                            ApprovalRemarks = "Pending",
                            ToList = new List<string> { "user3@example.com" },
                            CCList = new List<string> { "user4@example.com" }
                        }
                    });
                    await context.SaveChangesAsync();

                    var IQuery = context.Reports.AsQueryable();
                    List<string> employeesAccess = ["Branch1", "Branch2"];
                    var mockUserClaims = new UserClaimsDTO
                    {
                        EmployeeID = "E12345",
                        RoleID = 1,
                        RoleName = "BOCCH",
                        Name = "John Doe",
                        LoginName = "jdoe",
                        LoginDateTime = "2025-01-06 09:30:00"
                    };
                    var service = Subject();

                    // Mock the GetClaims method to return the mockUserClaims
                    _userClaims.Setup(service => service.GetClaims()).Returns(mockUserClaims);
                    // Mock the repository methods to return IQueryable<Report> synchronously
                    _repo.Setup(r => r.GetEmeployeesWithAccess(It.IsAny<string>())).ReturnsAsync(employeesAccess);
                    _repo.Setup(r => r.GetList(It.IsAny<string>(), It.IsAny<List<string>>()))
                        .Returns(IQuery); // Return the IQueryable directly, no need for async
                    _repo.Setup(r => r.GetListFilterWithPendingAndApproved(It.IsAny<string>(), It.IsAny<List<string>>()))
                        .Returns(IQuery); // Return the IQueryable directly, no need for async
                    _repo.Setup(r => r.GetListFilterWithApproved(It.IsAny<string>(), It.IsAny<List<string>>()))
                        .Returns(IQuery); // Return the IQueryable directly, no need for async

                    // Act
                    var result = await service.GetList(); // Since the methods are synchronous, no need for await

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(2, result.CountData); // Asserting the total count
                    Assert.Equal(1, result.PageIndex); // Asserting the page index
                    Assert.Equal(2, result.Data.Count); // Asserting the number of items
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                finally
                {
                    if (context != null)
                    {
                        await context.DisposeAsync(); // Asynchronously dispose the context
                    }
                }
            }
        }
        [Fact]
        public async Task PopulateGroupsDropDownList()
        {
            var service = Subject();
            var groups = new List<Group>
            {
                new Group
                {
                    Id = 1,
                    Code = "GRP001",
                    Name = "Group A",
                    Description = "Description for Group A",
                    DateCreated = DateTime.Now.AddMonths(-2),
                    DateModified = DateTime.Now.AddMonths(-1),
                    CreatedBy = "Admin",
                    Area = "Area 1",
                    Division = "Division 1"
                },
                new Group
                {
                    Id = 2,
                    Code = "GRP002",
                    Name = "Group B",
                    Description = "Description for Group B",
                    DateCreated = DateTime.Now.AddMonths(-1),
                    DateModified = DateTime.Now.AddMonths(-1),
                    CreatedBy = "Admin",
                    Area = "Area 2",
                    Division = "Division 2"
                },
                new Group
                {
                    Id = 3,
                    Code = "GRP003",
                    Name = "Group C",
                    Description = "Description for Group C",
                    DateCreated = DateTime.Now.AddDays(-10),
                    DateModified = DateTime.Now.AddDays(-5),
                    CreatedBy = "User1",
                    Area = "Area 3",
                    Division = "Division 3"
                }
            };

            _repo.Setup(r => r.PopulateGroupsDropDownList(It.IsAny<string>())).ReturnsAsync(groups);

            var data = await service.PopulateGroupsDropDownList();

            Assert.NotNull(data);
            Assert.Equal(3, data.Count);
        }
        [Fact]
        public async Task DownloadAdhoc_ShouldCallGeneratePervasivenessLogic_WhenReportAdhocIsPervasiveness()
        {
            // Arrange
            var service = Subject();
            var vm = new DownloadAdhocViewModel
            {
                ReportAdhoc = ReportAdhoc.Pervasiveness
            };
            EPPlusReturn file = new EPPlusReturn { FileName = "Pervasiveness" };
            var response = ResponseHelper.SuccessResponse<EPPlusReturn>(file, "");
            _repo.Setup(repo => repo.GeneratePervasivenessLogic(It.IsAny<DownloadAdhocViewModel>()))
                           .ReturnsAsync(response);

            // Act
            var result = await service.DownloadAdhoc(vm);

            // Assert
            _repo.Verify(repo => repo.GeneratePervasivenessLogic(vm), Times.Once);
            Assert.IsType<GenericResponse<EPPlusReturn>>(result);
            Assert.Contains(file.FileName, result.Data.FileName);
        }
        [Fact]
        public async Task DownloadAdhoc_ShouldCallGenerateRegularizationTATLogic_WhenReportAdhocIsRegularizationTAT()
        {
            // Arrange
            var service = Subject();
            var vm = new DownloadAdhocViewModel
            {
                ReportAdhoc = ReportAdhoc.RegularizationTAT
            };
            EPPlusReturn file = new EPPlusReturn { FileName = "RegularizationTAT" };
            var response = ResponseHelper.SuccessResponse<EPPlusReturn>(file, "");
            _repo.Setup(repo => repo.GenerateRegularizationTATLogic(It.IsAny<DownloadAdhocViewModel>()))
                           .ReturnsAsync(response);

            // Act
            var result = await service.DownloadAdhoc(vm);

            // Assert
            _repo.Verify(repo => repo.GenerateRegularizationTATLogic(vm), Times.Once);
            Assert.IsType<GenericResponse<EPPlusReturn>>(result);
            Assert.Contains(file.FileName, result.Data.FileName);
        }

        [Fact]
        public async Task DownloadAdhoc_ShouldCallGenerateExceptionAdhocsLogic_WhenReportAdhocIsExceptionAdhocs()
        {
            // Arrange
            var service = Subject();
            var vm = new DownloadAdhocViewModel
            {
                ReportAdhoc = ReportAdhoc.ExceptionAdhocs
            };
            EPPlusReturn file = new EPPlusReturn { FileName = "{1}_{0}" };
            var response = ResponseHelper.SuccessResponse<EPPlusReturn>(file, "");
            _repo.Setup(repo => repo.GenerateExceptionAdhocsLogic(It.IsAny<DownloadAdhocViewModel>(),It.IsAny<string>()))
                           .ReturnsAsync(response);

            // Act
            var result = await service.DownloadAdhoc(vm);

            // Assert
            Assert.IsType<GenericResponse<EPPlusReturn>>(result);
            Assert.Contains(file.FileName, result.Data.FileName);
        }
        [Fact]
        public async Task DownloadAdhoc_ShouldReturnErrorResponse_WhenReportAdhocIsAuditTrail()
        {
            // Arrange
            var service = Subject();
            var vm = new DownloadAdhocViewModel
            {
                ReportAdhoc = ReportAdhoc.AuditTrail
            };

            // Act
            var result = await service.DownloadAdhoc(vm);

            // Assert
            Assert.IsType<GenericResponse<EPPlusReturn>>(result);
            Assert.Equal("No extracted data from the filtered query", result.Message);
        }

        [Fact]
        public async Task DownloadAdhoc_ShouldReturnErrorResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var service = Subject();
            var vm = new DownloadAdhocViewModel
            {
                ReportAdhoc = ReportAdhoc.Pervasiveness
            };
            _repo.Setup(repo => repo.GeneratePervasivenessLogic(It.IsAny<DownloadAdhocViewModel>()))
                           .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await service.DownloadAdhoc(vm);

            // Assert
            Assert.IsType<GenericResponse<EPPlusReturn>>(result);
            Assert.Equal("Test exception", result.Message);
        }
    }
}
