using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.Helpers;
using Application.Services;
using Application.Services.Application.Services;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection InfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CcemQatContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default"),
              b => b.MigrationsAssembly(typeof(ServiceContainer).Assembly.FullName)),
              ServiceLifetime.Scoped);


            // Configure JWT Authentication
            var jwtSettings = configuration.GetSection("Jwt");
            var key = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new ArgumentNullException("JWT configuration is missing or incomplete.");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            });



            services.AddHttpContextAccessor();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IBranchCodeRepository, BranchCodeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportDetailsRepository, ReportDetailsRepository>();
            services.AddScoped<IReportBranchReplyRepository, ReportBranchReplyRepository>();
            services.AddScoped<IReportGenerateRepository, ReportGenerateRepository>();
            services.AddScoped<IExceptionsMgmtRepository, ExceptionsMgmtRepository>();
            services.AddScoped<ISubExceptionsRepository, SubExceptionsRepository>();


            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IBranchCodeService, BranchCodeService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReportDetailsService, ReportDetailsService>();
            services.AddScoped<IReportBranchReplyService, ReportBranchReplyService>();
            services.AddScoped<IReportGenerateService, ReportGenerateService>();
            services.AddScoped<IExceptionsMgmtService, ExceptionsMgmtService>();
            services.AddScoped<ISubExceptionsService, SubExceptionsService>();
            services.AddScoped<IAuditLogService, AuditLogService>();

            services.AddSingleton<IUserClaimsService,UserClaimsService>();

            services.AddScoped<EPPlusPackages>();
            services.AddScoped<Notification>();
            services.AddScoped<Utilities>();
            //services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            //services.AddScoped<IReportsRepository, ReportsRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();

            services.AddHttpContextAccessor();
            //services.AddSingleton<UserClaimsGetter>();

            services.AddSingleton<Logs>();

            return services;

        }


    }
}
