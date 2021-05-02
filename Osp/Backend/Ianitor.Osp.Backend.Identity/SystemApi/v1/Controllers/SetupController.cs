using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Infrastructure.CredentialGenerator;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ianitor.Osp.Backend.Identity.SystemApi.v1.Controllers
{
  [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
  [Route(IdentityServiceConstants.ApiPathPrefix + "/[controller]")]
  [ApiController]
  [ApiVersion(IdentityServiceConstants.ApiVersion1)]
  public class SetupController : ControllerBase
  {
    private readonly UserManager<OspUser> _userManager;
    private readonly RoleManager<OspRole> _roleManager;
    private readonly ILogger<SetupController> _logger;
    private readonly ICredentialGenerator _credentialGenerator;

    public SetupController(UserManager<OspUser> userManager, RoleManager<OspRole> roleManager,
      ILogger<SetupController> logger,
      ICredentialGenerator credentialGenerator)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _logger = logger;
      _credentialGenerator = credentialGenerator;
    }

    /// <summary>
    /// Configures identity services in the case no user is existing.
    /// </summary>
    /// <param name="adminUserDto">The client to be added. A client with the same client id must not exist.</param>
    /// <response code="200">Returns the created client.</response>
    /// <response code="400">The client could not be created due to either invalid input or failure to replace
    /// the client when another client with the same clientId already exists.</response>
    /// <response code="404">Not Found. The setting of the admin user is allowed only during installation.</response>
    [HttpPost]
    public async Task<IActionResult> AddAdminUser([FromBody] AdminUserDto adminUserDto)
    {
      if (_userManager.Users.Any())
      {
        return NotFound("The request is not valid for this configuration.");
      }

      if (!_credentialGenerator.CheckPassword(adminUserDto.Password))
      {
        _logger.LogInformation("The password does not comply with the minimum requirements.");
        return StatusCode(StatusCodes.Status406NotAcceptable);
      }

      var adminRole = await _roleManager.FindByNameAsync(CommonConstants.AdministratorsRole);
      if (adminRole == null)
      {
        _logger.LogInformation("No Administrator-Role has been found.");
        return StatusCode(StatusCodes.Status406NotAcceptable);
      }

      var adminUser = await _userManager.FindByNameAsync(adminUserDto.EMail);
      if (adminUser == null)
      {
        adminUser = new OspUser {UserName = adminUserDto.EMail, Email = adminUserDto.EMail};

        await _userManager.CreateAsync(adminUser, adminUserDto.Password);
        await _userManager.AddToRoleAsync(adminUser, adminRole.Id.ToString());
      }

      return Ok();
    }
  }
}
