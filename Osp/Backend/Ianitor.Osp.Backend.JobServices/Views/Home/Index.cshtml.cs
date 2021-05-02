#pragma warning disable 1591
using System.Text;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.DistributedCache;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ianitor.Osp.Backend.JobServices.Views.Home
{
    public class IndexModel : PageModel
    {
        private readonly IDistributedWithPubSubCache _cache;

        public IndexModel(IDistributedWithPubSubCache cache)
        {
            _cache = cache;
        }

        public string CachedTimeUTC { get; set; }

        public async Task OnGetAsync()
        {
            CachedTimeUTC = "Cached Time Expired";
            var encodedCachedTimeUTC = (byte[]) await _cache.Database.StringGetAsync("cachedTimeUTC");

            if (encodedCachedTimeUTC != null)
            {
                CachedTimeUTC = Encoding.UTF8.GetString(encodedCachedTimeUTC);
            }
        }
    }
}