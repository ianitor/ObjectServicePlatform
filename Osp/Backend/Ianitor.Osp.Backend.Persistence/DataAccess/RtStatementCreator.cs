using System;
using System.Linq;
using Ianitor.Osp.Backend.Persistence.CkRuleEngine.Cache;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Backend.Persistence.Formulas;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    internal class RtStatementCreator : RtStatementCreator<RtEntity>
    {
        internal RtStatementCreator(EntityCacheItem entityCacheItem, IDatabaseContext databaseContext, string language) 
            : base(entityCacheItem, databaseContext, language)
        {
        }
    }
    
    internal class RtStatementCreator<TEntity> : StatementCreator<TEntity> where TEntity : RtEntity, new()
    {
        private readonly EntityCacheItem _entityCacheItem;

        internal RtStatementCreator(EntityCacheItem entityCacheItem, IDatabaseContext databaseContext, string language)
            : base(databaseContext.GetRtCollection<TEntity>(entityCacheItem.CkId), language)
        {
            _entityCacheItem = entityCacheItem;
        }

        protected override bool IsAttributeNameValid(string attributeName)
        {
            return _entityCacheItem.Attributes.TryGetValue(attributeName, out AttributeCacheItem _) ||
                   attributeName == nameof(RtEntity.WellKnownName);
        }

        protected override string ResolveAttributeName(string attributeName)
        {
            if (typeof(RtEntity).GetProperty(attributeName) != null)
            {
                return attributeName.ToCamelCase();
            }

            return $"{Constants.AttributesName}.{attributeName.ToCamelCase()}";
        }

        protected override string GetEntityName()
        {
            return _entityCacheItem.CkId;
        }

        protected override object ResolveSearchAttributeValue(string attributeName, object searchTerm, out bool isEnum)
        {
            if (searchTerm != null && _entityCacheItem.Attributes.TryGetValue(attributeName, out AttributeCacheItem attributeCacheItem))
            {
                if (attributeCacheItem.SelectionValues != null)
                {
                    var searchTermString = searchTerm.ToString();

                    // Search for match in selection value
                    var result = attributeCacheItem.SelectionValues.FirstOrDefault(x =>
                        string.Equals(x.Name, searchTermString, StringComparison.OrdinalIgnoreCase));
                    if (result != null)
                    {
                        isEnum = false;
                        return result.Key;
                    }
                }

                if (searchTerm.ToString().StartsWith("@"))
                {
                    var expression = new OspExpression(searchTerm.ToString().Substring(1));
                    var result = expression.calculate();

                    if (double.IsNegativeInfinity(result))
                    {
                        isEnum = false;
                        return null;
                    }
                    if (!double.IsNaN(result))
                    {
                        switch (attributeCacheItem.AttributeValueType)
                        {
                            case AttributeValueTypes.DateTime:
                                isEnum = false;
                                return new DateTime((long) result);
                        }
                    }
                    else
                    {
                        throw new OperationFailedException($"Term '{searchTerm}' cannot be evaluated by formula.");
                    }
                }
                
                // Change to the type of attribute
                isEnum = false;
                return RtEntity.ConvertAttributeValue(attributeCacheItem.AttributeValueType, searchTerm);
            }

            return base.ResolveSearchAttributeValue(attributeName, searchTerm, out isEnum);
        }
    }
}