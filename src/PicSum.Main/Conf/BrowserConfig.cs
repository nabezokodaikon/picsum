using System.Drawing;
using System.Windows.Forms;

namespace PicSum.Main.Conf
{
    /// <summary>
    /// ブラウザ設定
    /// </summary>
    internal sealed class BrowserConfig
    {
        public static readonly BrowserConfig Instance = new();

        public FormWindowState WindowState { get; set; }
        public Point WindowLocaion { get; set; }
        public Size WindowSize { get; set; }

        private BrowserConfig() { }
    }
}
