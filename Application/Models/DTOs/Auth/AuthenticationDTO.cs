using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Auth
{
    public class AuthenticationDTO
    {
        public class AuthRequest
        {
            public string? Username { get; set; }
            public string? Password { get; set; }

        }

        public class AuthResponse
        {
            public bool IsAuthenticated { get; }
            public string Message { get; }
            public string Token { get; }

            public AuthResponse(bool success, string message, string? token = null)
            {
                IsAuthenticated = success;
                Message = message;
                Token = token;
            }
        }
    }
}
