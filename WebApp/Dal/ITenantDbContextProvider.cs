using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Dal
{
    public interface ITenantDbContextProvider
    {
        Task<TenantDbContext> Get();
    }
}
