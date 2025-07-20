using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public class BaseForm
        : Form
    {
        public bool IsLoaded { get; private set; } = false;

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

            this.Load += this.BaseForm_Load;
        }

        private void BaseForm_Load(object? sender, EventArgs e)
        {
            this.IsLoaded = true;
        }
    }
}
