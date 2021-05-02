using Ianitor.Osp.Common.Shared.DataTransferObjects;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ianitor.Osp.Backend.Identity.SystemApi.v1.Controllers
{
    /// <summary>
    /// REST Controller for diagnostics information
    /// </summary>
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route(IdentityServiceConstants.ApiPathPrefix + "/[controller]")]
    [ApiController]
    [ApiVersion(IdentityServiceConstants.ApiVersion1)]
    public class DiagnosticsController : ControllerBase
    {
        /// <summary>
        /// Returns a diagnostics information of the current authenticated user 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {           
            var model = new DiagnosticsDto(HttpContext.User);
            return Ok(model);
        }
    }
}