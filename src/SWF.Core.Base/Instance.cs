namespace SWF.Core.Base
{
    public static class Instance<TValue>
    {
#pragma warning disable CS8618
        public static TValue Value { get; private set; }
#pragma warning restore CS8618

        public static void Initialize(TValue connection)
        {
            ArgumentNullException.ThrowIfNull(connection, nameof(connection));
            Value = connection;
        }
    }
}