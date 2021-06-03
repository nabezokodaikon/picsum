using System.Drawing;
using System.Windows.Forms;

namespace PicSum.Main.Conf
{
    /// <summary>
    /// ブラウザ設定
    /// </summary>
    public static class BrowserConfig
    {
        public static FormWindowState WindowState { get; set; }
        public static Point WindowLocaion { get; set; }
        public static Size WindowSize { get; set; }
    }
}
