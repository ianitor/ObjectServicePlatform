using System.Threading.Tasks;
using Ianitor.Osp.Backend.Identity.ViewModels.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ianitor.Osp.Backend.Identity.Controllers.Diagnostics
{
  [Authorize]
  public class DiagnosticsController : Controller
  {
    public async Task<IActionResult> Index()
    {
      var model = new DiagnosticsViewModel(await HttpContext.AuthenticateAsync());
      return View(model);
    }
  }
}
