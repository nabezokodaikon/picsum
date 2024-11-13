namespace SWF.Core.FileAccessor
{
    public sealed class FileAppender
    {
        private static readonly int FILE_READ_BUFFER_SIZE = 1024 * 4;

        public FileAppender()
        {

        }

        public byte[] Read(string filePath, int startPoint, int size)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (var fs = new FileStream(
                filePath, FileMode.Open, FileAccess.Read, FileShare.Read, FILE_READ_BUFFER_SIZE, FileOptions.RandomAccess))
            {
                var bf = new byte[size];
                fs.Seek(startPoint, SeekOrigin.Begin);
                fs.ReadExactly(bf, 0, size);
                return bf;
            }
        }

        public int Append(string filePath, byte[] buffer)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (var fs = new FileStream(
                filePath, FileMode.Append, FileAccess.Write, FileShare.None, FILE_READ_BUFFER_SIZE, FileOptions.None))
            using (var bs = new BufferedStream(fs, FILE_READ_BUFFER_SIZE))
            {
                var offset = (int)fs.Length;
                bs.Write(buffer, 0, buffer.Length);
                return offset;
            }
        }
    }
}
