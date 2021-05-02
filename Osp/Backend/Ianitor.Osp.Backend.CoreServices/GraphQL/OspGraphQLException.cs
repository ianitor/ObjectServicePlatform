using System;
using System.Runtime.Serialization;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.CoreServices.GraphQL
{
    [Serializable]
    public class OspGraphQLException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public OspGraphQLException()
        {
        }

        public OspGraphQLException(string message) : base(message)
        {
        }

        public OspGraphQLException(string message, Exception inner) : base(message, inner)
        {
        }

        protected OspGraphQLException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}