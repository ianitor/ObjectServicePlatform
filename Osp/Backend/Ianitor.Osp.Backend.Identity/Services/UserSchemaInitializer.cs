using System.Threading.Tasks;
using Ianitor.Osp.Backend.Infrastructure.Initialization;

namespace Ianitor.Osp.Backend.Identity.Services
{
  public class UserSchemaInitializer : IAsyncInitializationService
  {
    private readonly IUserSchemaService _userSchemaService;

    public UserSchemaInitializer(IUserSchemaService userSchemaService)
    {
      _userSchemaService = userSchemaService;
    }

    public async Task InitializeAsync()
    {
      await _userSchemaService.SetupAsync();
    }
  }
}
