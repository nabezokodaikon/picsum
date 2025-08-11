namespace SWF.Core.Base
{
    public static class Instance<TValue>
        where TValue : class
    {
        private static Lazy<TValue>? _lazyValue = null;

        public static TValue Value
        {
            get
            {
                if (_lazyValue == null)
                {
                    throw new InvalidOperationException("インスタンスが設定されていません。");
                }

                return _lazyValue.Value;
            }
        }

        public static void Initialize(Lazy<TValue> lazyValue)
        {
            ArgumentNullException.ThrowIfNull(lazyValue, nameof(lazyValue));

            if (_lazyValue != null)
            {
                throw new InvalidOperationException("インスタンスが既に設定されています。");
            }

            AppConstants.ThrowIfNotUIThread();

            _lazyValue = lazyValue;
        }
    }
}
