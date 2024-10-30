using SWF.Core.Base;
using SWF.Core.FileAccessor;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PicSum.Main.Conf
{
    [SupportedOSPlatform("windows")]
    public sealed class Config
    {
        public static readonly Config Values = new();

        private static readonly string CONFIG_FILE
            = Path.Combine(FileUtil.CONFIG_DIRECTORY, "config.xml");

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
            using (var fs = new FileStream(CONFIG_FILE, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(fs))
            {
                serializer.Serialize(writer, this);
            }
        }

        public void Load()
        {
            if (FileUtil.IsExists(CONFIG_FILE))
            {
                var serializer = new XmlSerializer(typeof(Config));
                Config saveData = null;
                using (var fs = new FileStream(CONFIG_FILE, FileMode.Open, FileAccess.Read, FileShare.Read))
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
        }
    }
}
