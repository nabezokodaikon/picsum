namespace SWF.Core.ResourceAccessor
{
    public interface IFileIconCacher
        : IDisposable
    {
        public Image SmallPCIcon { get; }
        public Image LargePCIcon { get; }
        public Image SmallDirectoryIcon { get; }
        public Image ExtraLargeDirectoryIcon { get; }
        public Image JumboDirectoryIcon { get; }

        public Image GetSmallDriveIcon(string filePath);
        public Image GetExtraLargeDriveIcon(string filePath);
        public Image GetJumboDriveIcon(string filePath);
        public Image GetSmallFileIcon(string filePath);
        public Image GetExtraLargeFileIcon(string filePath);
        public Bitmap GetJumboFileIcon(string filePath);
    }
}
