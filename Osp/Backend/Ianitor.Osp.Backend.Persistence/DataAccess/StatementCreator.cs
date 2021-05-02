using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.Formulas;
using Ianitor.Osp.Common.Shared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    public abstract class StatementCreator<TEntity> where TEntity : class, new()
    {
        private readonly ICachedCollection<TEntity> _cachedCollection;

        private readonly List<FilterDefinition<TEntity>> _idFilters;
        private readonly List<FilterDefinition<TEntity>> _fieldFilters;
        private readonly List<FilterDefinition<TEntity>> _attributeSearchFilter;

        private readonly List<SortDefinition<TEntity>> _sortDefinitions;
        private readonly BsonClassMap<TEntity> _bsonClassMap;

        internal StatementCreator(ICachedCollection<TEntity> cachedCollection, string language = "en")
        {
            _cachedCollection = cachedCollection;

            Language = language;

            _idFilters = new List<FilterDefinition<TEntity>>();
            _fieldFilters = new List<FilterDefinition<TEntity>>();
            _attributeSearchFilter = new List<FilterDefinition<TEntity>>();
            _sortDefinitions = new List<SortDefinition<TEntity>>();

            _bsonClassMap = new BsonClassMap<TEntity>();
            _bsonClassMap.AutoMap();
        }

        public string Language { get; }

        protected virtual bool IsAttributeNameValid(string attributeName)
        {
            var memberMap = _bsonClassMap.GetMemberMap(attributeName);
            return memberMap != null;
        }

        protected virtual string ResolveAttributeName(string attributeName)
        {
            if (_bsonClassMap.IdMemberMap.MemberName == attributeName)
            {
                return Constants.IdField;
            }

            var memberMap = _bsonClassMap.GetMemberMap(attributeName);
            return memberMap.ElementName;
        }

        protected virtual string GetEntityName()
        {
            return typeof(TEntity).Name;
        }

        public async Task<ResultSet<TEntity>> ExecuteQuery(IOspSession ospSession, int? skip = null, int? take = null)
        {
            using var performanceMonitor = new PerformanceMonitor();

            var filters = new List<FilterDefinition<TEntity>>();
            if (_attributeSearchFilter.Any())
            {
                if (_attributeSearchFilter.Count > 1)
                {
                    filters.Add(Builders<TEntity>.Filter.Or(_attributeSearchFilter));
                }
                else
                {
                    filters.Add(_attributeSearchFilter.First());
                }
            }
            filters.AddRange( _idFilters.Concat(_fieldFilters));


            var aggregate = CreateAggregate(ospSession);
            

            // In documentation, text search must be at first place
            if (_textFilter != null)
            {
                aggregate = aggregate.Match(_textFilter);
                aggregate = aggregate.Sort(Builders<TEntity>.Sort.MetaTextScore("score"));
            }

            if (filters.Any())
            {
                var filterDefinition = Builders<TEntity>.Filter.Empty;
                if (filters.Any())
                {
                    if (filters.Count == 1)
                    {
                        filterDefinition = filters.First();
                    }
                    else
                    {
                        filterDefinition = Builders<TEntity>.Filter.And(filters);
                    }
                }
                
                aggregate = aggregate.Match(filterDefinition);
            }

            if (_sortDefinitions.Any())
            {
                var sortDefinition = Builders<TEntity>.Sort.Combine(_sortDefinitions);
                aggregate = aggregate.Sort(sortDefinition);
            }

            performanceMonitor.SetCheckPoint("definitions created");

            // Return result directly if there is no paging enabled
            if (!take.HasValue && !skip.HasValue)
            {
                var resultNoTotalCount = await aggregate.ToListAsync();
                return new ResultSet<TEntity>(resultNoTotalCount, resultNoTotalCount.Count);
            }


            var pagingDefinitionList = new List<BsonDocument>();

            if (skip.HasValue)
            {
                pagingDefinitionList.Add(new BsonDocument
                {
                    {
                        "$skip", skip.Value
                    }
                });
            }
            
            if (take.HasValue)
            {
                pagingDefinitionList.Add(new BsonDocument
                {
                    {
                        "$limit", take.Value
                    }
                });
            }

            var countDefinition = new BsonDocument
            {
                {
                    "$count", "count"
                }
            };

            var resultAggregate = aggregate.Facet<StatementQueryResult<TEntity>>(new[]
                {
                    new AggregateFacet<TEntity, StatementQueryResult<TEntity>>("result",
                        PipelineDefinition<TEntity, StatementQueryResult<TEntity>>.Create(pagingDefinitionList)),
                    new AggregateFacet<TEntity, StatementQueryResult<TEntity>>("totalCount",
                        PipelineDefinition<TEntity, StatementQueryResult<TEntity>>.Create(countDefinition))
                }
            );

            var result = await resultAggregate.SingleOrDefaultAsync();
            return new ResultSet<TEntity>(result);
        }

        protected virtual IAggregateFluent<TEntity> CreateAggregate(IOspSession ospSession)
        {
            var aggregate = _cachedCollection.Aggregate(ospSession);
            return aggregate;
        }

        internal void AddIdFilter<TField>(IReadOnlyList<TField> ids)
        {
            if (ids == null || !ids.Any())
            {
                return;
            }

            _idFilters.Add(Builders<TEntity>.Filter.In(Constants.IdField, ids));
        }

        internal void AddFieldFilters(IEnumerable<FieldFilter> fieldFilters)
        {
            if (fieldFilters == null)
            {
                return;
            }

            foreach (var fieldFilter in fieldFilters)
            {
                AddFieldFilter(fieldFilter);
            }
        }

        private void AddFieldFilter(FieldFilter fieldFilter)
        {
            if (string.IsNullOrWhiteSpace(fieldFilter.AttributeName))
            {
                return;
            }

            if (IsAttributeNameValid(fieldFilter.AttributeName) ||
                fieldFilter.AttributeName == Constants.IdField)
            {
                var resolvedAttributeName = ResolveAttributeName(fieldFilter.AttributeName);
                var resolvedValue = ResolveSearchAttributeValue(fieldFilter.AttributeName, fieldFilter.ComparisonValue,
                    out bool isEnum);

                if (isEnum)
                {
                    _fieldFilters.Add(Builders<TEntity>.Filter.AnyIn(resolvedAttributeName,
                        (IEnumerable<object>) resolvedValue));
                }
                else
                {
                    var filter = CreateFilter(resolvedAttributeName, fieldFilter.Operator, resolvedValue);
                    _fieldFilters.Add(filter);
                }
            }
            else
            {
                throw new OperationFailedException(
                    $"Attribute '{fieldFilter.AttributeName}' does not exist on type '{GetEntityName()}'");
            }
        }

        protected virtual object ResolveSearchAttributeValue(string attributeName, object searchTerm, out bool isEnum)
        {
            if (searchTerm == null)
            {
                isEnum = false;
                return null;
            }

            var propertyType = typeof(TEntity).GetProperty(attributeName)?.PropertyType;
            if (propertyType != null && propertyType.IsEnum)
            {
                var nameCandidates = Enum.GetNames(propertyType)
                    .Where(x => x.ToLower().Contains(searchTerm.ToString().ToLower()));

                List<object> values = new List<object>();
                foreach (var nameCandidate in nameCandidates)
                {
                    values.Add(Enum.Parse(propertyType, nameCandidate));
                }

                isEnum = true;
                return values.ToArray();
            }

            if (searchTerm.ToString().StartsWith("@"))
            {
                var expression = new OspExpression(searchTerm.ToString().Substring(1));
                var result = expression.calculate();

                if (!double.IsNaN(result))
                {
                    if (propertyType == typeof(DateTime))
                    {
                        isEnum = false;
                        return new DateTime((long) result);
                    }
                }
                else
                {
                    throw new OperationFailedException($"Term '{searchTerm}' cannot be evaluated by formula.");
                }
            }

            if (propertyType != null && searchTerm is string)
            {
                isEnum = false;

                try
                {
                    return Convert.ChangeType(searchTerm, propertyType);
                }
                catch (Exception)
                {
                    // Indented to not handle exception
                }
            }

            isEnum = false;
            return searchTerm;
        }

        internal void AddTextSearchFilter(TextSearchFilter textSearchFilter)
        {
            if (textSearchFilter?.SearchTerm == null)
            {
                return;
            }

            _textFilter = Builders<TEntity>.Filter.Text(textSearchFilter.SearchTerm.ToString(), new TextSearchOptions
            {
                CaseSensitive = false,
                Language = Language,
                DiacriticSensitive = true
            });
        }

        private FilterDefinition<TEntity> _textFilter;

        internal void AddAttributeSearchFilter(AttributeSearchFilter attributeSearchFilter)
        {
            if (attributeSearchFilter?.SearchTerm == null || attributeSearchFilter.AttributeNames == null ||
                !attributeSearchFilter.AttributeNames.Any())
            {
                return;
            }


            // ReSharper disable once PossibleMultipleEnumeration
            var attributeNameList = attributeSearchFilter.AttributeNames.ToList();

            foreach (var attributeName in attributeNameList)
            {
                if (IsAttributeNameValid(attributeName))
                {
                    var resolvedAttributeName = ResolveAttributeName(attributeName);
                    var resolvedValue = ResolveSearchAttributeValue(attributeName,
                        attributeSearchFilter.SearchTerm, out bool isEnum);

                    if (isEnum)
                    {
                        _attributeSearchFilter.Add(Builders<TEntity>.Filter.AnyIn(resolvedAttributeName,
                            (IEnumerable<object>) resolvedValue));
                    }
                    else
                    {
                        _attributeSearchFilter.Add(CreateFilter(resolvedAttributeName, FieldFilterOperator.Like,
                            resolvedValue));
                    }
                }
                else
                {
                    throw new OperationFailedException(
                        $"Attribute '{attributeName}' does not exist on type '{GetEntityName()}'");
                }
            }
        }

        internal void AddSort(IEnumerable<SortOrderItem> sortOrders)
        {
            if (sortOrders == null)
                return;

            var sortOrderList = sortOrders.ToList();
            if (!sortOrderList.Any())
            {
                return;
            }

            foreach (var item in sortOrderList)
            {
                if (!IsAttributeNameValid(item.AttributeName) && item.AttributeName != Constants.IdField)
                {
                    throw new OperationFailedException(
                        $"Sort definition contains attribute '{item.AttributeName}', but attribute does not exist on type '{GetEntityName()}'");
                }

                var resolvedAttributeName = ResolveAttributeName(item.AttributeName);

                switch (item.SortOrder)
                {
                    case SortOrders.Ascending:
                        _sortDefinitions.Add(Builders<TEntity>.Sort.Ascending(resolvedAttributeName));
                        break;
                    case SortOrders.Descending:
                        _sortDefinitions.Add(Builders<TEntity>.Sort.Descending(resolvedAttributeName));
                        break;
                    default:
                        continue;
                }
            }
        }

        private FilterDefinition<TEntity> CreateFilter(string attributeName, FieldFilterOperator comparisonOperator,
            object value)
        {
            switch (comparisonOperator)
            {
                case FieldFilterOperator.Equals:
                    return Builders<TEntity>.Filter.Eq(attributeName, value);
                case FieldFilterOperator.NotEquals:
                    return Builders<TEntity>.Filter.Ne(attributeName, value);
                case FieldFilterOperator.In:
                    return Builders<TEntity>.Filter.In(attributeName, (IEnumerable<object>) value);
                case FieldFilterOperator.NotIn:
                    return Builders<TEntity>.Filter.Nin(attributeName, (IEnumerable<object>) value);
                case FieldFilterOperator.LessThan:
                    return Builders<TEntity>.Filter.Lt(attributeName, value);
                case FieldFilterOperator.LessEqualThan:
                    return Builders<TEntity>.Filter.Lte(attributeName, value);
                case FieldFilterOperator.GreaterThan:
                    return Builders<TEntity>.Filter.Gt(attributeName, value);
                case FieldFilterOperator.GreaterEqualThan:
                    return Builders<TEntity>.Filter.Gte(attributeName, value);
                case FieldFilterOperator.Like:
                    return Builders<TEntity>.Filter.Regex(attributeName,
                        new BsonRegularExpression(GetRegex(value?.ToString()), "i"));
                case FieldFilterOperator.MatchRegEx:
                    return Builders<TEntity>.Filter.Regex(attributeName,
                        new BsonRegularExpression(value?.ToString()));
                default:
                    throw new NotImplementedException("Value is not implemented.");
            }
        }

        private static string GetRegex(string value)
        {
            return value?.Replace("*", "/");
        }
    }
}