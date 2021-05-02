using System;
using System.Linq;

namespace Ianitor.Osp.Frontend.Client
{

    public static class Extensions
    {
        public static Uri Append(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) =>
                $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));
        }
    }
}
