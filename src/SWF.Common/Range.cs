using System.Collections;
using System.Collections.Generic;

namespace SWF.Common
{
    public class Range : IEnumerable<int>
    {
        private int _from;
        private int _to;

        public Range(int from, int to)
        {
            _from = from;
            _to = to;
        }

        private IEnumerator<int> GetEnumerator()
        {
            if (_from < _to)
            {
                for (int i = _from; i <= _to; i++)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = _from; i >= _to; i--)
                {
                    yield return i;
                }
            }
        }

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
