using System.Threading.Tasks;
using Ianitor.Osp.Backend.Infrastructure.Initialization;

namespace Ianitor.Osp.Backend.Authentication.DynamicAuth
{
  public class DynamicAuthSchemeServiceInitializer : IAsyncInitializationService
  {
    private readonly IDynamicAuthSchemeService _dynamicAuthSchemeService;

    public DynamicAuthSchemeServiceInitializer(IDynamicAuthSchemeService dynamicAuthSchemeService)
    {
      _dynamicAuthSchemeService = dynamicAuthSchemeService;
    }
    public async Task InitializeAsync()
    {
      await _dynamicAuthSchemeService.ConfigureAsync();
    }
  }
}
