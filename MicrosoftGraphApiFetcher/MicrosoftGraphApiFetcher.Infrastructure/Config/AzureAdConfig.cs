namespace MicrosoftGraphApiFetcher.Infrastructure.Config
{
    /// <summary>
    /// The Azure AD configuration to access your tenant.
    /// </summary>
    public class AzureAdConfig
    {
        public required string AppId { get; set; }
        public required string Secret { get; set; }
        public required string TenantId { get; set; }
    }
}