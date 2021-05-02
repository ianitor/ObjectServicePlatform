using GraphQL.Types;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Types
{
    /// <summary>
    /// Implements the GraphQL type used for subscription messages
    /// </summary>
    public class DynamicUpdateMessageDtoType<TItemType> : ObjectGraphType<DynamicUpdateMessageDto<TItemType>> where TItemType : GraphQLDto
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemType">The GraphQL type used as item type.</param>
        public DynamicUpdateMessageDtoType(IGraphType itemType)
        {
            Name = $"{itemType.Name}{CommonConstants.GraphQlUpdateMessageSuffix}";
            this.Field("Items", "The corresponding items", graphType: new ListGraphType(itemType));
        }
    }
}