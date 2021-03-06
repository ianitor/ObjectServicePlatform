using System;
using System.Runtime.Serialization;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    [Serializable]
    public class InvalidCkIdException : OperationFailedException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidCkIdException()
        {
        }

        public InvalidCkIdException(string message) : base(message)
        {
        }

        public InvalidCkIdException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidCkIdException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}