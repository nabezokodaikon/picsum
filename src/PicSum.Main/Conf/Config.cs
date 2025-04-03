using SWF.Core.Base;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PicSum.Main.Conf
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class Config
    {
        public static readonly Config Instance = new();

        public FormWindowState WindowState { get; set; }
        public Point WindowLocaion { get; set; }
        public Size WindowSize { get; set; }
        public string ExportDirectoryPath { get; set; }
        public int ThumbnailSize { get; set; }
        public bool IsShowFileName { get; set; }
        public bool IsShowDirectory { get; set; }
        public bool IsShowImageFile { get; set; }
        public bool IsShowOtherFile { get; set; }
        public int FavoriteDirectoryCount { get; set; }
        public ImageDisplayMode ImageDisplayMode { get; set; }
        public ImageSizeMode ImageSizeMode { get; set; }

        private Config()
        {

        }

        public void Save()
        {
            var serializer = new XmlSerializer(typeof(Config));
            using (var fs = new FileStream(AppConstants.CONFIG_FILE, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(fs))
            {
                serializer.Serialize(writer, this);
            }
        }

        public void Load()
        {
            ConsoleUtil.Write(true, $"Config.Load Start");

            if (FileUtil.IsExistsFile(AppConstants.CONFIG_FILE))
            {
                var serializer = new XmlSerializer(typeof(Config));
                Config saveData = null;
                using (var fs = new FileStream(AppConstants.CONFIG_FILE, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new StreamReader(fs))
                {
                    saveData = (Config)serializer.Deserialize(reader);
                }

                this.WindowState = saveData.WindowState; ;
                this.WindowLocaion = saveData.WindowLocaion;
                this.WindowSize = saveData.WindowSize;
                this.ThumbnailSize = saveData.ThumbnailSize;
                this.IsShowFileName = saveData.IsShowFileName;
                this.IsShowImageFile = saveData.IsShowImageFile;
                this.IsShowDirectory = saveData.IsShowDirectory;
                this.IsShowOtherFile = saveData.IsShowOtherFile;
                this.FavoriteDirectoryCount = saveData.FavoriteDirectoryCount;
                this.ImageDisplayMode = saveData.ImageDisplayMode;
                this.ImageSizeMode = saveData.ImageSizeMode;
                this.ExportDirectoryPath = saveData.ExportDirectoryPath;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;

                var primaryScreenBounds = Screen.PrimaryScreen.Bounds;
                this.WindowLocaion = new Point(
                    (int)(primaryScreenBounds.Width * 0.3),
                    (int)(primaryScreenBounds.Height * 0.1));

                this.WindowSize = new Size(1200, 800);

                this.ThumbnailSize = 144;
                this.IsShowFileName = true;
                this.IsShowImageFile = true;
                this.IsShowDirectory = true;
                this.IsShowOtherFile = true;
                this.FavoriteDirectoryCount = 21;
                this.ImageDisplayMode = ImageDisplayMode.LeftFacing;
                this.ImageSizeMode = ImageSizeMode.FitOnlyBigImage;

                this.ExportDirectoryPath
                    = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }

            ConsoleUtil.Write(true, $"Config.Load End");
        }
    }
}
