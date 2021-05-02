using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.DataAccess;

namespace Ianitor.Osp.Backend.Persistence.MongoDb
{
  public interface IRepositoryClient
  {
    Task CreateRepositoryAsync(string name);

    Task DropRepositoryAsync(string name);

    IRepository GetRepository(string name);

    Task<bool> IsRepositoryExistingAsync(string name);

    Task  CreateUser(IOspSession session, string authenticationDatabaseName, string databaseName, string user, string password);
  }
}
