using System;
using System.Runtime.Serialization;

namespace Ianitor.Osp.Frontend.Client
{
    [Serializable]
    public class ServiceConfigurationMissingException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ServiceConfigurationMissingException()
        {
        }

        public ServiceConfigurationMissingException(string message) : base(message)
        {
        }

        public ServiceConfigurationMissingException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ServiceConfigurationMissingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}