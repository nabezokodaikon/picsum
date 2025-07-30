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

            if (Thread.CurrentThread.Name != AppConstants.UI_THREAD_NAME)
            {
                throw new InvalidOperationException("UIスレッド以外から呼び出されました。");
            }

            _lazyValue = lazyValue;
        }
    }
}
