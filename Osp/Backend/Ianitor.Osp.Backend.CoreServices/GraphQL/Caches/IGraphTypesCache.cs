using GraphQL.Types;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Types;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Caches
{
    /// <summary>
    /// Interface of the graph type cache 
    /// </summary>
    public interface IGraphTypesCache
    {
        
        /// <summary>
        /// Gets or creates an RtEntityDtoType based on an entity id key
        /// </summary>
        /// <param name="ckId">The entity id key.</param>
        /// <returns>The cached RtEntityDtoType based on the given entity id key</returns>
        RtEntityDtoType GetOrCreate(string ckId);
        
        /// <summary>
        /// Gets or creates an RtEntityDtoInputType based on an entity id key
        /// </summary>
        /// <param name="ckId">The entity id key.</param>
        /// <returns>The cached RtEntityDtoType based on the given entity id key</returns>
        RtEntityDtoInputType GetOrCreateInput(string ckId);

        /// <summary>
        /// Gets or creates a Connection Type based on a given GraphQL type
        /// </summary>
        /// <param name="graphType">The GraphQL type</param>
        /// <param name="prefixName">The prefix of the name of the connection</param>
        /// <returns>ConnectionType that corresponds to the given GraphQL type</returns>
        DynamicConnectionType GetOrCreateConnection(IGraphType graphType, string prefixName);

        /// <summary>
        /// Returns an array of known graph types
        /// </summary>
        /// <returns></returns>
        IGraphType[] GetTypes();
    }
}