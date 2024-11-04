namespace SWF.Core.FileAccessor
{
    public sealed class FileAppender
    {
        private readonly int bufferSize;

        public FileAppender(int bufferSize)
        {
            this.bufferSize = bufferSize;
        }

        public byte[] Read(string filePath, int startPoint, int size)
        {
            using (var fs = new FileStream(
                filePath, FileMode.Open, FileAccess.Read, FileShare.Read, this.bufferSize, FileOptions.RandomAccess))
            {
                var bf = new byte[size];
                fs.Seek(startPoint, SeekOrigin.Begin);
                fs.Read(bf, 0, size);
                return bf;
            }
        }

        public int Append(string filePath, byte[] buffer)
        {
            using (var fs = new FileStream(
                filePath, FileMode.Append, FileAccess.Write, FileShare.None, this.bufferSize, FileOptions.None))
            using (var bs = new BufferedStream(fs, this.bufferSize))
            {
                var offset = (int)fs.Length;
                bs.Write(buffer, 0, buffer.Length);
                return offset;
            }
        }
    }
}
