using Application.Contracts.Services;
using Application.Models.DTOs.Common;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Services
{

    namespace Application.Services
    {
        public class UserClaimsService : IUserClaimsService
        {
            private readonly IHttpContextAccessor _httpContext;
            public UserClaimsService(IHttpContextAccessor httpContext)
            {
                _httpContext = httpContext;
            }
            public UserClaimsDTO GetClaims()
            {
                var httpContext = _httpContext.HttpContext;
                var userClaims = new UserClaimsDTO();

                if (httpContext.User.Identity != null && httpContext.User.Identity.IsAuthenticated)
                {
                    var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();

                    //if (!string.IsNullOrWhiteSpace(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                    //{
                        var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;

                        if (token != null)
                        {
                            // Read custom claims from the token
                            var employeeId = token.Claims.FirstOrDefault(c => c.Type == "EmployeeID")?.Value;
                            var loginName = token.Claims.FirstOrDefault(c => c.Type == "LoginName")?.Value;
                            var name = token.Claims.FirstOrDefault(c => c.Type == "Name")?.Value;
                            var role = token.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
                            var loginDateTime = token.Claims.FirstOrDefault(c => c.Type == "LoginDateTime")?.Value;

                            // Assign values to the DTO
                            userClaims = new UserClaimsDTO()
                            {
                                EmployeeID = employeeId,
                                LoginName = loginName,
                                Name = name,
                                RoleName = role,
                                LoginDateTime = loginDateTime
                            };
                        }


                    //}
                }
                return userClaims;
            }
           
        }
    }

}
