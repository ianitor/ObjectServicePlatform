using System.ComponentModel.DataAnnotations;

namespace Ianitor.Osp.Backend.Persistence.SystemEntities
{
  /// <summary>
  /// Identity provider configuration specifically for Google accounts.
  /// </summary>
  public class GoogleIdentityProvider : OspIdentityProvider
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public GoogleIdentityProvider()
    {
      Type = IdentityProviderTypes.Google;
    }

    /// <summary>
    /// client id
    /// </summary>
    [Required]
    public string ClientId { get; set; }

    /// <summary>
    /// client secret
    /// </summary>
    [Required]
    public string ClientSecret { get; set; }
  }
}
