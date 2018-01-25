using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Dal
{
    public class UserRepository : IUserRepository
    {
        private readonly ITenantDbContextProvider _provider;

        public UserRepository(ITenantDbContextProvider provider)
        {
            _provider = provider;
        }

        async Task<User> IUserRepository.Add(User user)
        {
            var ctx = await _provider.Get();
            await ctx.Users.AddAsync(user);
            await ctx.SaveChangesAsync();
            return user;
        }

        async Task<User> IUserRepository.GetByUpnAndTenantId(string upn, Guid tenantId)
        {
            var ctx = await _provider.Get();
            return await ctx.Users.Where(u => u.PrincipalName == upn && u.TenantId == tenantId).FirstOrDefaultAsync();
        }
    }
}
