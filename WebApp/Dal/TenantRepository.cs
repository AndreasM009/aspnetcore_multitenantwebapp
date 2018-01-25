using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Dal
{
    public class TenantRepository : ITenantRepository
    {
        private readonly ITenantDbContextProvider _provider;
        
        public TenantRepository(ITenantDbContextProvider provider)
        {
            _provider = provider;
        }

        async Task<Tenant> ITenantRepository.Add(Tenant tenant)
        {
            var ctx = await _provider.Get();

            tenant.Id = Guid.NewGuid();
            await ctx.Tenants.AddAsync(tenant);
            await ctx.SaveChangesAsync();
            return tenant;
        }

        async Task<Tenant> ITenantRepository.GetByStateMarker(string stateMarker)
        {
            var ctx = await _provider.Get();

            return await ctx.Tenants.Where(t => t.StateMarker == stateMarker).FirstOrDefaultAsync();
        }

        async Task<Tenant> ITenantRepository.GetByTenantId(Guid tenantId)
        {
            var ctx = await _provider.Get();
            return await ctx.Tenants.Where(t => t.TenantId == tenantId).FirstOrDefaultAsync();
        }

        async Task<Tenant> ITenantRepository.Remove(Tenant tenant)
        {
            var ctx = await _provider.Get();
            ctx.Tenants.Remove(tenant);
            await ctx.SaveChangesAsync();
            return tenant;
        }

        async Task<Tenant> ITenantRepository.Update(Tenant tenant)
        {
            var ctx = await _provider.Get();

            ctx.Tenants.Update(tenant);
            await ctx.SaveChangesAsync();
            return tenant;
        }
    }
}
