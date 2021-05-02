using System;
using System.Linq;

namespace Ianitor.Osp.Common.Shared
{
  public static class UriExtensions
  {
    /// <summary>
    /// Adds the path part to the URI without losing the path of the primary URI.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="paths"></param>
    /// <returns></returns>
    /// <example>
    /// "http://ex.com/base/api" + "/extended/api" returns "http://ex.com/base/api/extended/api"
    /// </example>
    public static Uri Append(this Uri uri, params string[] paths)
    {
      return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) =>
        $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));
    }
  }
}
