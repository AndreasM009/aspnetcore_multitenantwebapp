using System;

namespace WebApp.Dal
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string StateMarker { get; set; }
        public bool AdminConsented { get; set; }
        public string Issuer { get; set; }
    }
}
