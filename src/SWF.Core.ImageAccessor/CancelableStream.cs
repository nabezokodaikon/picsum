namespace SWF.Core.ImageAccessor
{
    internal sealed partial class CancelableStream
        : Stream
    {
        private readonly Stream innerStream;
        private readonly Action checkCancelAction;

        public CancelableStream(FileStream innerStream, Action checkCancelAction)
        {
            ArgumentNullException.ThrowIfNull(innerStream, nameof(innerStream));
            ArgumentNullException.ThrowIfNull(checkCancelAction, nameof(checkCancelAction));

            this.innerStream = innerStream;
            this.checkCancelAction = checkCancelAction;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                this.checkCancelAction();
            }
            catch
            {
                return 0;
            }

            return innerStream.Read(buffer, offset, count);
        }

        public override bool CanRead => this.innerStream.CanRead;
        public override bool CanSeek => this.innerStream.CanSeek;
        public override bool CanWrite => this.innerStream.CanWrite;
        public override long Length => this.innerStream.Length;
        public override long Position
        {
            get => this.innerStream.Position;
            set => this.innerStream.Position = value;
        }

        public override void Flush() => this.innerStream.Flush();
        public override long Seek(long offset, SeekOrigin origin) => this.innerStream.Seek(offset, origin);
        public override void SetLength(long value) => this.innerStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => this.innerStream.Write(buffer, offset, count);
    }
}
