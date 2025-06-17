namespace SWF.Core.Base
{
    public sealed class FastLazy<TValue>
        where TValue : class
    {
        private readonly Lock _lock = new();
        private readonly Func<TValue> _valueFactory;
        private volatile TValue? _value = null;

        public TValue Value
        {
            get
            {
                if (this._value == null)
                {
                    lock (this._lock)
                    {
                        this._value ??= this._valueFactory();
                    }
                }

                return this._value;
            }
        }

        public FastLazy(Func<TValue> valueFactory)
        {
            ArgumentNullException.ThrowIfNull(valueFactory, nameof(valueFactory));

            this._valueFactory = valueFactory;
        }

        public void Dispose()
        {
            lock (this._lock)
            {
                var instance = this._value as IDisposable;
                instance?.Dispose();

            }
        }
    }
}
