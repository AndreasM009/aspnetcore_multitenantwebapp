using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Dal
{
    public class User
    {
        public string PrincipalName { get; set; }
        public Guid TenantId { get; set; }
    }
}
