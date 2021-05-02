using System;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL.Utils
{
    public struct EdgeRange
    {
        private int _startOffset;
        private int _endOffset;

        public EdgeRange(int startOffset, int endOffset)
        {
            if (startOffset < 0) throw new ArgumentOutOfRangeException(nameof(startOffset));
            if (endOffset < -1) throw new ArgumentOutOfRangeException(nameof(endOffset));
            this._startOffset = startOffset;
            this._endOffset = Math.Max(startOffset - 1, endOffset);
        }

        public int StartOffset => _startOffset;

        public int EndOffset => _endOffset;

        public int Count => _endOffset - _startOffset + 1;

        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Ensures that <see cref="Count"/> is equal to or less than <paramref name="maxLength"/>
        /// by moving the end offset towards start offset. 
        /// </summary>
        /// <param name="maxLength">Maximum count</param>
        public void LimitCountFromStart(int maxLength)
        {
            if (maxLength < 0) throw new ArgumentOutOfRangeException(nameof(maxLength));
            if (maxLength < Count)
            {
                _endOffset = _startOffset + maxLength - 1;
            }
        }
        
        
        /// <summary>
        /// Ensures that <see cref="Count"/> is equal to or less than <paramref name="maxLength"/>
        /// by moving the start offset towards start offset. 
        /// </summary>
        /// <param name="maxLength">Maximum count</param>
        public void LimitCountToEnd(int maxLength)
        {
            if (maxLength < 0) throw new ArgumentOutOfRangeException(nameof(maxLength));
            if (maxLength < Count)
            {
                _startOffset = _endOffset - maxLength + 1;
            }
        }
    }
}