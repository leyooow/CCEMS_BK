using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IAuthRepository
    {
        Task<User> GetUserByLoginNameAsync(string loginName);
        Task UpdateUserAsync(User user);

        Task SaveLoginAuditLogAsync(User user, string loginName);
        Task LogOutAsync();
    }
}
