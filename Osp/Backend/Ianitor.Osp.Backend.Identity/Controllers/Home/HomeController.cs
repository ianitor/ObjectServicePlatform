using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Identity.ViewModels.Home;
using Ianitor.Osp.Backend.Identity.ViewModels.Shared;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable 1591

namespace Ianitor.Osp.Backend.Identity.Controllers.Home
{
    [SecurityHeaders]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserManager<OspUser> _userManager;

        public HomeController(IIdentityServerInteractionService interaction, UserManager<OspUser> userManager)
        {
            _interaction = interaction;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!_userManager.Users.Any())
            {
                return RedirectToAction("Index", "Setup");
            }

            if (!User.IsAuthenticated())
            {
                return Redirect("~/Account/Login");
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                // Same cookie - but ids to not match - new database?
                return Redirect("~/Account/Login");
            }

            var vm = new HomeViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EMail = user.Email,
                UserName = user.UserName,
                AccessFailedCount = user.AccessFailedCount,
                Id = user.Id.ToString(),
            };

            return View(vm);
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Path?.StartsWith($"/{IdentityServiceConstants.ApiPathPrefix}") ?? false)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            var vm = new ErrorViewModel();

            // retrieve error details from identity server
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return View("Error", vm);
        }

        private Task<OspUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}