using System;
using Ianitor.Common.Shared;

namespace Ianitor.Osp.Backend.Persistence
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CkIdAttribute : Attribute
    {
        public CkIdAttribute(string ckId)
        {
            ArgumentValidation.ValidateString(nameof(ckId), ckId);

            CkId = ckId;
        }
        
        public string CkId { get; }
    }
}