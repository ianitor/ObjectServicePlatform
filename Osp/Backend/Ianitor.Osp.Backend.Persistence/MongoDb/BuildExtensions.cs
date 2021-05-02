using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.MongoDb
{
    internal static class BuildExtensions
    {
        internal static FilterDefinition<TDocument> BuildIdFilter<TDocument, TField>(this FilterDefinitionBuilder<TDocument> _this, TField id)
        {
            return _this.Eq(Constants.IdField, id);
        }
    }
}