using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;

namespace Ianitor.Osp.Backend.Persistence.MongoDb
{
  /// <summary>
  /// Basic interface with a few methods for adding, deleting, and querying data.
  /// </summary>
  public interface IRepository
  {
    Task<IOspSession> StartSessionAsync();
    

    Task CreateCollectionIfNotExistsAsync<TCollection>(string suffix = null) where TCollection: class, new();


    ICachedCollection<T> GetCollection<T>(string suffix = null) where T : class, new();
  }
}
