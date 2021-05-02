using System;

namespace Ianitor.Osp.Backend.Persistence.SystemStores
{
    [Serializable]
    public class NotExistingException : Exception
    {
        public NotExistingException() { }
        public NotExistingException(string message) : base(message) { }
        public NotExistingException(string message, Exception inner) : base(message, inner) { }
        protected NotExistingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
