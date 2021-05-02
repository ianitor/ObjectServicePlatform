using System;
using System.Runtime.Serialization;

namespace Ianitor.Osp.Backend.Jobs
{
    /// <summary>
    /// Represents an exception when a job fails
    /// </summary>
    [Serializable]
    public class JobFailedException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <inheritdoc />
        public JobFailedException()
        {
        }

        /// <inheritdoc />
        public JobFailedException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public JobFailedException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc />
        protected JobFailedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}