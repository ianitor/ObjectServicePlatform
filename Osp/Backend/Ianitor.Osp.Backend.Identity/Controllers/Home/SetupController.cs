using System;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Identity.ViewModels.Setup;
using Ianitor.Osp.Backend.Infrastructure.CredentialGenerator;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Ianitor.Osp.Common.Internationalization;
using Ianitor.Osp.Common.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ianitor.Osp.Backend.Identity.Controllers.Home
{
  [SecurityHeaders]
  public class SetupController : Controller
  {
    private readonly ILogger<SetupController> _logger;
    private readonly UserManager<OspUser> _userManager;
    private readonly RoleManager<OspRole> _roleManager;
    private readonly ICredentialGenerator _credentialGenerator;

    public SetupController(ILogger<SetupController> logger, UserManager<OspUser> userManager,
      RoleManager<OspRole> roleManager, ICredentialGenerator credentialGenerator)
    {
      _logger = logger;
      _userManager = userManager;
      _roleManager = roleManager;
      _credentialGenerator = credentialGenerator;
    }

    public IActionResult Index()
    {
      if (_userManager.Users.Any())
      {
        return RedirectToAction("Index", "Home");
      }

      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(SetupViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      if (_userManager.Users.Any())
      {
        ModelState.AddModelError(String.Empty, Texts.Backend_Identity_Setup_Status_UsersAlreadyConfigured);
        return View(model);
      }

      if (!_credentialGenerator.CheckPassword(model.NewPassword))
      {
        ModelState.AddModelError(String.Empty, Texts.Backend_Identity_Setup_Status_PasswordComplexity);
        return View(model);
      }

      var adminRole = await _roleManager.FindByNameAsync(CommonConstants.AdministratorsRole);
      if (adminRole == null)
      {
        _logger.LogInformation("No Administrator-Role has been found.");

        ModelState.AddModelError(String.Empty, Texts.Backend_General_Error_Label);
        return View(model);
      }

      var adminUser = await _userManager.FindByNameAsync(model.EMailAddress);
      if (adminUser == null)
      {
        adminUser = new OspUser {UserName = model.EMailAddress, Email = model.EMailAddress};

        await _userManager.CreateAsync(adminUser, model.NewPassword);
        await _userManager.AddToRoleAsync(adminUser, adminRole.NormalizedName);
      }

      return RedirectToAction("Index", "Home");
    }
  }
}
