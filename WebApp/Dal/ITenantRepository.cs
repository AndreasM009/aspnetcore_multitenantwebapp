using System;
using System.Threading.Tasks;

namespace WebApp.Dal
{
    public interface ITenantRepository
    {
        Task<Tenant> GetByStateMarker(string stateMarker);
        Task<Tenant> Add(Tenant tenant);
        Task<Tenant> Update(Tenant tenant);
        Task<Tenant> GetByTenantId(Guid tenantId);
        Task<Tenant> Remove(Tenant tenant);
    }
}
