using MessagePack;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.ResourceAccessor;
using System;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Main.Conf
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    [MessagePackObject(AllowPrivate = true)]
    public sealed partial class Config
    {
        public static readonly Config INSTANCE = new();

        [Key(0)]
        public FormWindowState WindowState { get; set; }

        [Key(1)]
        public int WindowLocaionX { get; set; }

        [Key(2)]
        public int WindowLocaionY { get; set; }

        [Key(3)]
        public int WindowSizeWidth { get; set; }

        [Key(4)]
        public int WindowSizeHeight { get; set; }

        [Key(5)]
        public int ThumbnailSize { get; set; }

        [Key(6)]
        public bool IsShowFileName { get; set; }

        [Key(7)]
        public bool IsShowDirectory { get; set; }

        [Key(8)]
        public bool IsShowImageFile { get; set; }

        [Key(9)]
        public bool IsShowOtherFile { get; set; }

        [Key(10)]
        public int FavoriteDirectoryCount { get; set; }

        [Key(11)]
        public ImageDisplayMode ImageDisplayMode { get; set; }

        [Key(12)]
        public ImageSizeMode ImageSizeMode { get; set; }

        [Key(13)]
        public int MajorVersion { get; set; }

        [Key(14)]
        public int MinorVersion { get; set; }

        [Key(15)]
        public int BuildVersion { get; set; }

        [Key(16)]
        public int RevisionVersion { get; set; }

        private Config()
        {

        }

        public void Load()
        {
            using (TimeMeasuring.Run(true, "Config.Load"))
            {
                if (FileUtil.IsExistsFile(AppFiles.CONFIG_FILE.Value))
                {
                    var config = MessagePackSerializer.Deserialize<Config>(
                        File.ReadAllBytes(AppFiles.CONFIG_FILE.Value),
                        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.None));

                    this.WindowState = config.WindowState;
                    this.WindowLocaionX = config.WindowLocaionX;
                    this.WindowLocaionY = config.WindowLocaionY;
                    this.WindowSizeWidth = config.WindowSizeWidth;
                    this.WindowSizeHeight = config.WindowSizeHeight;
                    this.ThumbnailSize = Math.Min(config.ThumbnailSize, ThumbnailUtil.THUMBNAIL_MAXIMUM_SIZE);
                    this.IsShowFileName = config.IsShowFileName;
                    this.IsShowImageFile = config.IsShowImageFile;
                    this.IsShowDirectory = config.IsShowDirectory;
                    this.IsShowOtherFile = config.IsShowOtherFile;
                    this.FavoriteDirectoryCount = config.FavoriteDirectoryCount;
                    this.ImageDisplayMode = config.ImageDisplayMode;
                    this.ImageSizeMode = config.ImageSizeMode;

                    this.MajorVersion = config.MajorVersion;
                    this.MinorVersion = config.MinorVersion;
                    this.BuildVersion = config.BuildVersion;
                    this.RevisionVersion = config.RevisionVersion;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;

                    var primaryScreenBounds = Screen.PrimaryScreen.Bounds;
                    this.WindowLocaionX = (int)(primaryScreenBounds.Width * 0.3);
                    this.WindowLocaionY = (int)(primaryScreenBounds.Height * 0.1);

                    this.WindowSizeWidth = 1200;
                    this.WindowSizeHeight = 800;

                    this.ThumbnailSize = 144;
                    this.IsShowFileName = true;
                    this.IsShowImageFile = true;
                    this.IsShowDirectory = true;
                    this.IsShowOtherFile = true;
                    this.FavoriteDirectoryCount = 21;
                    this.ImageDisplayMode = ImageDisplayMode.LeftFacing;
                    this.ImageSizeMode = ImageSizeMode.FitOnlyBigImage;

                    this.MajorVersion = 0;
                    this.MinorVersion = 0;
                    this.BuildVersion = 0;
                    this.RevisionVersion = 0;
                }
            }
        }

        public void Save()
        {
            this.MajorVersion = AppInfo.CURRENT_VERSION.Major;
            this.MinorVersion = AppInfo.CURRENT_VERSION.Minor;
            this.BuildVersion = AppInfo.CURRENT_VERSION.Build;
            this.RevisionVersion = AppInfo.CURRENT_VERSION.Revision;

            var bytes = MessagePackSerializer.Serialize(
                this,
                MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.None));

            File.WriteAllBytes(AppFiles.CONFIG_FILE.Value, bytes);
        }

        public string GetOldVersion()
        {
            return $"{this.MajorVersion}.{this.MinorVersion}.{this.BuildVersion}.{this.RevisionVersion}";
        }

        public string GetCurrentVersion()
        {
            return $"{AppInfo.CURRENT_VERSION.Major}.{AppInfo.CURRENT_VERSION.Minor}.{AppInfo.CURRENT_VERSION.Build}.{AppInfo.CURRENT_VERSION.Revision}";
        }
    }
}
