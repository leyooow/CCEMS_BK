using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Services
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateToken(User user);
    }
}
