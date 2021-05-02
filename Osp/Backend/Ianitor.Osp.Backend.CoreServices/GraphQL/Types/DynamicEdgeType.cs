using GraphQL.Types;
using Ianitor.Common.Shared;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    /// <summary>
    /// Implements an edge GraphQL type for dynamic creation (without using generic types - because we create types based on data source settings!)
    /// </summary>
    public class DynamicEdgeType : ObjectGraphType<object>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the edge type</param>
        /// <param name="description">Description of the edge type</param>
        /// <param name="nodeType">The node type of the edge type</param>
        public DynamicEdgeType(string name, string description, IGraphType nodeType)
        {
            ArgumentValidation.ValidateString(nameof(name), name);
            ArgumentValidation.ValidateString(nameof(description), description);

            Name = name;
            Description = description;
            Field<NonNullGraphType<StringGraphType>>().Name("cursor").Description("A cursor for use in pagination");
            
            this.Field("node", "The item at the end of the edge", graphType: nodeType);
        }
    }
}