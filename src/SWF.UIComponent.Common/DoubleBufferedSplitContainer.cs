using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SWF.UIComponent.Common
{
    [SupportedOSPlatform("windows")]
    public class DoubleBufferedSplitContainer
        : SplitContainer
    {
        public DoubleBufferedSplitContainer()
        {
            //this.SetStyle(
            //    ControlStyles.OptimizedDoubleBuffer |
            //    ControlStyles.AllPaintingInWmPaint,
            //    true);

            //this.UpdateStyles();
        }
    }
}
