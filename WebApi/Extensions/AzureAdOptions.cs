namespace Microsoft.AspNetCore.Authentication
{
    public class AzureAdOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiUri { get; set; }
        public string Instance { get; set; }
    }
}
