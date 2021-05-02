﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Ianitor.Osp.Backend.Persistence
{
    [Serializable]
    public class PersistenceException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public PersistenceException()
        {
        }

        public PersistenceException(string message) : base(message)
        {
        }

        public PersistenceException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PersistenceException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
