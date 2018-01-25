﻿namespace Microsoft.AspNetCore.Authentication
{
    public class AzureAdOptions
    {
        public string ClientId { get; set; }
        
        public string ClientSecret { get; set; }
        
        public string Instance { get; set; }
        
        public string Domain { get; set; }
        
        public string CallbackPath { get; set; }

        public string GraphApiUri { get; set; }

        public string WebApiUri { get; set; }

        public string SignUpRequestUri { get; set; }
    }
}
