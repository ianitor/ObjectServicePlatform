using System;
using System.Text;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.JobServices.Views.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.JobServices.Controllers.Home
{
    public class HomeController : Controller
    {
        private readonly IDistributedWithPubSubCache _distributedCache;

        public HomeController(IDistributedWithPubSubCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        
        // GET
        public async Task<IActionResult> Index()
        {
            var model = new IndexModel(_distributedCache);
            await model.OnGetAsync();
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> OnPostResetCachedTime()
        {
            var currentTimeUTC = DateTime.UtcNow.ToString();
            byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(currentTimeUTC);
            await _distributedCache.Database.StringSetAsync("cachedTimeUTC", encodedCurrentTimeUTC);

            return RedirectToAction("Index");
        }
    }
}