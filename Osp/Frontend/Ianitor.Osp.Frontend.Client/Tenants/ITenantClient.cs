using System.Threading.Tasks;
using GraphQL;

namespace Ianitor.Osp.Frontend.Client.Tenants
{
    public interface ITenantClient : IServiceClient
    {
        TenantClientOptions Options { get; }

        Task<QlItemsContainer<TDto>> SendQueryAsync<TDto>(GraphQLRequest query) where TDto : class;
        Task<TDto> SendMutationAsync<TDto>(GraphQLRequest query);
    }
}