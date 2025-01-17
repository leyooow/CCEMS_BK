using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Services.Application.Services;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.DirectoryServices.AccountManagement;
using static Application.Models.DTOs.Auth.AuthenticationDTO;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContext;
        

        public AuthService(IAuthRepository authRepository, IJwtTokenGenerator tokenGenerator, IAuditLogRepository auditLogRepository, IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            _authRepository = authRepository;
            _tokenGenerator = tokenGenerator;
            _auditLogRepository = auditLogRepository;
            _config = configuration;
            _httpContext = httpContext;
            
        }

        public async Task<AuthResponse> AuthenticateAsync(AuthRequest authRequest)
        {
            var user = await _authRepository.GetUserByLoginNameAsync(authRequest.Username);
            if (user == null)
                return new AuthResponse(false, "Invalid username or password.");

            if (user.IsLocked)
                return new AuthResponse(false, "User is locked. Please contact the admin.");

            var ipAddress = _httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString();
            if (user.Ipaddress != null && user.Ipaddress != ipAddress)
            {
                return new AuthResponse(false, "User is currently logged-in on another workstation.");
            }

            var ldapPath = _config["LDAPPath"];
            var lDapDomain = _config["LDAPDomain"];

            #region validating ad credentials

            //using (var context = new PrincipalContext(ContextType.Domain, "BOC"))
            //{
            //    // Validate credentials using the LDAP server
            //    if (!context.ValidateCredentials(authRequest.Username, authRequest.Password))
            //    {
            //        user.LogInCounter++;
            //        if (user.LogInCounter >= 3)
            //        {
            //            user.IsLocked = true;
            //            await _authRepository.UpdateUserAsync(user);
            //            return new AuthResponse(false, "User is locked due to multiple invalid login attempts.");
            //        }

            //        await _authRepository.UpdateUserAsync(user);
            //        return new AuthResponse(false, "Invalid username or password.");
            //    }
            //}

            #endregion

            //user.Ipaddress = ipAddress;
            user.LogInCounter = 0;
            user.LastLogIn = DateTime.UtcNow;

            await _authRepository.UpdateUserAsync(user);
            await _authRepository.SaveLoginAuditLogAsync(user, authRequest.Username);
            var token = await _tokenGenerator.GenerateToken(user);
            return new AuthResponse(true, "Login successful.", token);

        }

        public async Task<AuthResponse> LogoutAsync()
        {
            await _authRepository.LogOutAsync();
            return new AuthResponse(true, "Logout successfully.",null);
        }



    } 

}
