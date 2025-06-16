namespace SWF.Core.Base
{
    public static class Instance<TValue>
        where TValue : class
    {
        private static readonly Lock _lock = new();
        private static Func<TValue>? _valueFactory = null;
        private static volatile TValue? _value = null;

#pragma warning disable CS8602

        public static TValue Value
        {
            get
            {
                if (_value == null)
                {
                    lock (_lock)
                    {
                        _value ??= _valueFactory();
                    }
                }

                return _value;
            }
        }

#pragma warning restore CS8618

        public static void Initialize(Func<TValue> valueFactory)
        {
            ArgumentNullException.ThrowIfNull(valueFactory, nameof(valueFactory));

            _valueFactory = valueFactory;
        }
    }
}
