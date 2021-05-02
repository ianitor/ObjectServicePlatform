using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ianitor.Osp.Common.Shared;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.Backend.Dashboard.Controllers
{
    [AllowAnonymous]
    public class OidcConfigurationController : ControllerBase
    {
        private readonly OspDashboardOptions _ospDashboardOptions;

        public OidcConfigurationController(IOptions<OspDashboardOptions> ospDashboardOptions)
        {
            _ospDashboardOptions = ospDashboardOptions.Value;
        }

        [HttpGet("_configuration/{clientId}")]
        public IActionResult GetClientRequestParameters([FromRoute]string clientId)
        {
            if (clientId != CommonConstants.OspDashboardClientId)
            {
                return NotFound();
            }

            var clientDto = new ClientDto(clientId, _ospDashboardOptions);
            return Ok(clientDto);
        }
    }
}
