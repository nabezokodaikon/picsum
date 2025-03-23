namespace SWF.Core.Job
{
    public struct ValueParameter<T>
        : IJobParameter
    {
        public T Value { get; private set; }

        public ValueParameter(T value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            this.Value = value;
        }
    }
}
