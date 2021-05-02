using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using GraphQL;
using GraphQL.Builders;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Utils
{
    internal static class ResolveConnectionContextExtensions
    {
        internal static int? GetOffset<TEntity>(this IResolveConnectionContext<TEntity> ctx)
        {
            int? offset = null;
            if (!string.IsNullOrEmpty(ctx.After))
            {
                offset = ConnectionUtils.CursorToOffset(ctx.After) + 1;
            }

            return offset;
        }

        internal static bool TryGetArgument<TType>(this IResolveFieldContext context, string name, out TType value)
        {
            return TryGetArgument(context, name, default, out value);
        }
        
        internal static bool TryGetArgument<TType>(this IResolveFieldContext context, string name, TType defaultValue, out TType value)
        {
            if (context.HasArgument(name))
            {
                value = context.GetArgument<TType>(name);
                if (value != null)
                {
                    return true;
                }
            }

            value = defaultValue;
            return false;
        }
        
        internal static DataQueryOperation GetDataQueryOperation<TEntity>(this IResolveConnectionContext<TEntity> ctx)
        {
            DataQueryOperation dataQueryOperation = new DataQueryOperation();

            if (ctx.TryGetArgument(Statics.SearchFilterArg, out SearchFilterDto filterDto))
            {
                if (filterDto.Type == null || filterDto.Type == SearchFilterTypesDto.TextSearch)
                {
                    ArgumentValidation.ValidateString(nameof(filterDto.Language), filterDto.Language);

                    dataQueryOperation.Language = filterDto.Language;
                    dataQueryOperation.TextSearchFilter = new TextSearchFilter(filterDto.SearchTerm);
                }
                else
                {
                    ArgumentValidation.Validate(nameof(filterDto.AttributeNames), filterDto.AttributeNames);

                    dataQueryOperation.AttributeSearchFilter =
                        new AttributeSearchFilter(filterDto.AttributeNames?.Select(TransformAttributeName),
                            filterDto.SearchTerm);
                }
            }

            if (ctx.TryGetArgument(Statics.FieldFilterArg, out IEnumerable<FieldFilterDto> fieldFilterDtoList))
            {
                dataQueryOperation.FieldFilters = fieldFilterDtoList.Select(dto =>
                    new FieldFilter(TransformAttributeName(dto.AttributeName), (FieldFilterOperator) dto.Operator,
                        dto.ComparisonValue));
            }

            if (ctx.TryGetArgument(Statics.SortOrderArg, out IEnumerable<SortDto> sortDtos))
            {
                dataQueryOperation.SortOrders = sortDtos.Select(dto =>
                    new SortOrderItem(dto.AttributeName.ToPascalCase(), (SortOrders) dto.SortOrder));
            }

            return dataQueryOperation;
        }
        
        private static string TransformAttributeName(string attributeNameDto)
        {
            var attributeName = attributeNameDto.ToPascalCase();
            if (attributeName == nameof(RtEntityDto.RtId))
            {
                attributeName = Constants.IdField;
            }

            if (attributeName == nameof(RtEntityDto.WellKnownName))
            {
                attributeName = nameof(RtEntity.WellKnownName);
            }

            return attributeName;
        }
    }
}