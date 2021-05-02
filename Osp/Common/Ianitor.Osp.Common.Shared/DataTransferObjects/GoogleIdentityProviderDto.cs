using System.ComponentModel.DataAnnotations;

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
  /// <summary>
  /// Identity provider configuration specifically for Google accounts.
  /// </summary>
  public class GoogleIdentityProviderDto : IdentityProviderDto
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public GoogleIdentityProviderDto()
    {
      Type = IdentityProviderTypesDto.Google;
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
