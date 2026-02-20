namespace SWF.Core.ResourceAccessor
{
    public interface IFileIconCacher
        : IDisposable
    {
        public Bitmap SmallPCIcon { get; }
        public Image LargePCIcon { get; }
        public Bitmap SmallDirectoryIcon { get; }
        public Image ExtraLargeDirectoryIcon { get; }
        public Image JumboDirectoryIcon { get; }

        public Bitmap GetSmallDriveIcon(string filePath);
        public Image GetExtraLargeDriveIcon(string filePath);
        public Image GetJumboDriveIcon(string filePath);
        public Bitmap GetSmallFileIcon(string filePath);
        public Image GetExtraLargeFileIcon(string filePath);
        public Bitmap GetJumboFileIcon(string filePath);
    }
}
