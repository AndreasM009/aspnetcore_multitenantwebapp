using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication
{
    public class MemoryCacheTicketStore : ITicketStore
    {
        private const string KeyPrefix = "AuthSessionStore";
        private IMemoryCache _cache;

        public MemoryCacheTicketStore()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        Task ITicketStore.RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.FromResult(true);
        }

        Task ITicketStore.RenewAsync(string key, AuthenticationTicket ticket)
        {
            var options = new MemoryCacheEntryOptions();
            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue)
            {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            }
            options.SetSlidingExpiration(TimeSpan.FromHours(1));

            _cache.Set(key, ticket, options);

            return Task.FromResult(true);
        }

        Task<AuthenticationTicket> ITicketStore.RetrieveAsync(string key)
        {
            AuthenticationTicket ticket;
            _cache.TryGetValue(key, out ticket);
            return Task.FromResult(ticket);
        }

        async Task<string> ITicketStore.StoreAsync(AuthenticationTicket ticket)
        {
            var id = Guid.NewGuid();
            var key = KeyPrefix + id;
            await ((ITicketStore)this).RenewAsync(key, ticket);
            return key;
        }
    }
}
