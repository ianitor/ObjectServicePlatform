using System.ComponentModel.DataAnnotations;

namespace Ianitor.Osp.Backend.Persistence.SystemEntities
{
  /// <summary>
  /// Identity provider configuration specifically for Azure Active Directory.
  /// </summary>
  public class AzureAdIdentityProvider : OspIdentityProvider
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public AzureAdIdentityProvider()
    {
      Type = IdentityProviderTypes.AzureActiveDirectory;
    }

#pragma warning disable 1591
    public const string DefaultAuthority = "https://login.microsoftonline.com";
#pragma warning restore 1591

    /// <summary>
    /// The Tenant ID for the Azure Active Directory.
    /// </summary>
    [Required]
    public string TenantId { get; set; }

    /// <summary>
    /// Authority (default value: https://login.microsoftonline.com).
    /// </summary>
    [Required]
    public string Authority { get; set; } = DefaultAuthority;

    /// <summary>
    /// Client ID (group Azure AD).
    /// </summary>
    [Required]
    public string ClientIdGroupAzureAd { get; set; }

    /// <summary>
    /// Client Secret (group Azure AD).
    /// </summary>
    [Required]
    public string ClientSecretGroupAzureAd { get; set; }

    /// <summary>
    /// Client ID (group Graph API).
    /// </summary>
    [Required]
    public string ClientIdGroupGraphApi { get; set; }
  }
}
