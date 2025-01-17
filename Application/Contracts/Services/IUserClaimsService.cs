using Application.Models.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Services
{
    public interface IUserClaimsService
    {
        UserClaimsDTO GetClaims();
    }
}
