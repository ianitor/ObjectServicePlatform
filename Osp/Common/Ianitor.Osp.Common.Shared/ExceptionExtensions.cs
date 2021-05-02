using System;
using System.Text;

namespace Ianitor.Osp.Common.Shared
{
    public static class ExceptionExtensions
    {
        public static string GetDirectAndIndirectMessages(this Exception _this)
        {
            var stringBuilder = new StringBuilder();
            var prefix = "";

            Exception tmp = _this;
            while (tmp != null)
            {
                stringBuilder.AppendLine(prefix + tmp.Message);
                prefix += "\t";
                tmp = tmp.InnerException;
            }

            return stringBuilder.ToString();
        }
    }
}