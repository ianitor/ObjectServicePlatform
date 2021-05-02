using System;
using System.Runtime.Serialization;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.Identity.SystemApi.v1.Controllers
{
    [Serializable]
    public class RoleNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public RoleNotFoundException()
        {
        }

        public RoleNotFoundException(string message) : base(message)
        {
        }

        public RoleNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected RoleNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}