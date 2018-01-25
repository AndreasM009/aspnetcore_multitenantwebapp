using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Dal
{
    public interface IUserRepository
    {
        Task<User> GetByUpnAndTenantId(string upn, Guid tenantId);
        Task<User> Add(User user);
    }
}
