using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public class BaseForm
        : Form
    {
        public BaseForm()
        {
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.StandardClick,
                true);
            this.SetStyle(
                ControlStyles.ContainerControl |
                ControlStyles.Selectable,
                false);
            this.UpdateStyles();

            this.DoubleBuffered = true;
        }
    }
}
