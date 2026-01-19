using System.Collections;

namespace SWF.Core.Job
{
    public sealed class ListParameter<T>
        : IEnumerable<T>, IJobParameter
    {
        private readonly T[] _items;

        public ListParameter(T[] items)
        {
            this._items = items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in this._items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
