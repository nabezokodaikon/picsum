using MessagePack;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using SWF.Core.FileAccessor;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Main.Conf
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    [MessagePackObject(AllowPrivate = true)]
    public sealed partial class Config
    {
        public static readonly Config Instance = new();

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

        private Config()
        {

        }

        public void Load()
        {
            ConsoleUtil.Write(true, $"Config.Load Start");

            if (FileUtil.IsExistsFile(AppConstants.CONFIG_FILE.Value))
            {
                var config = MessagePackSerializer.Deserialize<Config>(
                    File.ReadAllBytes(AppConstants.CONFIG_FILE.Value),
                    MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.None));

                this.WindowState = config.WindowState;
                this.WindowLocaionX = config.WindowLocaionX;
                this.WindowLocaionY = config.WindowLocaionY;
                this.WindowSizeWidth = config.WindowSizeWidth;
                this.WindowSizeHeight = config.WindowSizeHeight;
                this.ThumbnailSize = config.ThumbnailSize;
                this.IsShowFileName = config.IsShowFileName;
                this.IsShowImageFile = config.IsShowImageFile;
                this.IsShowDirectory = config.IsShowDirectory;
                this.IsShowOtherFile = config.IsShowOtherFile;
                this.FavoriteDirectoryCount = config.FavoriteDirectoryCount;
                this.ImageDisplayMode = config.ImageDisplayMode;
                this.ImageSizeMode = config.ImageSizeMode;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;

                var primaryScreenBounds = Screen.PrimaryScreen.Bounds;
                this.WindowLocaionX = (int)(primaryScreenBounds.Width * 0.3);
                this.WindowLocaionY = (int)(primaryScreenBounds.Height * 0.1);

                this.WindowSizeWidth = 1200;
                this.WindowSizeHeight = 800;

                this.ThumbnailSize = 80;
                this.IsShowFileName = true;
                this.IsShowImageFile = true;
                this.IsShowDirectory = true;
                this.IsShowOtherFile = true;
                this.FavoriteDirectoryCount = 21;
                this.ImageDisplayMode = ImageDisplayMode.LeftFacing;
                this.ImageSizeMode = ImageSizeMode.FitOnlyBigImage;
            }

            ConsoleUtil.Write(true, $"Config.Load End");
        }

        public void Save()
        {
            var bytes = MessagePackSerializer.Serialize(
                this,
                MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.None));
            File.WriteAllBytes(AppConstants.CONFIG_FILE.Value, bytes);
        }
    }
}
