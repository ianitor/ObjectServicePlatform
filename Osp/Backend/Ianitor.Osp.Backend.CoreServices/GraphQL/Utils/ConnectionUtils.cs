using System.Collections.Generic;
using System.Linq;
using GraphQL.Builders;
using GraphQL.Types.Relay.DataObjects;
using Panic.StringUtils;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Utils
{
    internal static class ConnectionUtils
    {
        private const string Prefix = "arrayconnection";

        public static Connection<TSource> ToConnection<TSource, TParent>(
            IEnumerable<TSource> items,
            IResolveConnectionContext<TParent> context,
            bool strictCheck = true
        )
        {
            var list = items.ToList();
            return ToConnection(list, context, sliceStartIndex: 0, totalCount: list.Count, strictCheck: true);
        }

        public static Connection<TSource> ToConnection<TSource, TParent>(
            IEnumerable<TSource> slice,
            IResolveConnectionContext<TParent> context,
            int sliceStartIndex,
            int totalCount,
            bool strictCheck = true
        )
        {
            var sliceList = slice as IList<TSource> ?? slice.ToList();

            var metrics = ArraySliceMetrics.Create(
                sliceList,
                context,
                sliceStartIndex,
                totalCount,
                strictCheck
            );

            var edges = metrics.Slice.Select((item, i) => new Edge<TSource>
                {
                    Node = item,
                    Cursor = OffsetToCursor(metrics.StartOffset + i)
                })
                .ToList();

            var firstEdge = edges.FirstOrDefault();
            var lastEdge = edges.LastOrDefault();

            return new Connection<TSource>
            {
                Edges = edges,
                TotalCount = totalCount,
                PageInfo = new PageInfo
                {
                    StartCursor = firstEdge?.Cursor,
                    EndCursor = lastEdge?.Cursor,
                    HasPreviousPage = metrics.HasPrevious,
                    HasNextPage = metrics.HasNext,
                }
            };
        }

        public static string CursorForObjectInConnection<T>(
            IEnumerable<T> slice,
            T item
        )
        {
            var idx = slice.ToList().IndexOf(item);

            return idx == -1 ? null : OffsetToCursor(idx);
        }

        public static int CursorToOffset(string cursor)
        {
            return int.Parse(
                StringUtils.Base64Decode(cursor).Substring(Prefix.Length + 1)
            );
        }

        public static string OffsetToCursor(int offset)
        {
            return StringUtils.Base64Encode($"{Prefix}:{offset}");
        }

        public static int OffsetOrDefault(string cursor, int defaultOffset)
        {
            if (cursor == null)
                return defaultOffset;

            try
            {
                return CursorToOffset(cursor);
            }
            catch
            {
                return defaultOffset;
            }
        }
    }
}