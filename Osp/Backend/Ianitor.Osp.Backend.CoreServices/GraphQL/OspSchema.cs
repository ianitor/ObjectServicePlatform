using GraphQL;
using GraphQL.Types;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL
{
    /// <summary>
    /// Implements the Osp schema for a given data source
    /// </summary>
    public class OspSchema : Schema
    {
        static OspSchema()
        {
            ValueConverter.Register(typeof(string), typeof(OspObjectId), o => new OspObjectId((string) o));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ospQuery">The OSP query schema of a given data source</param>
        /// <param name="ospMutation"></param>
        /// <param name="ospSubscriptions"></param>
        public OspSchema(OspQuery ospQuery, OspMutation ospMutation, OspSubscriptions ospSubscriptions)
        {
            Query = ospQuery;
            Mutation = ospMutation;
            Subscription = ospSubscriptions;
        }
    }
}