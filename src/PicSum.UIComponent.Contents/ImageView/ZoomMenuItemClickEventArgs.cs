namespace PicSum.UIComponent.Contents.ImageView
{
    public sealed class ZoomMenuItemClickEventArgs
    {
        public float ZoomValue { get; private set; }

        public ZoomMenuItemClickEventArgs(float zoomValue)
        {
            this.ZoomValue = zoomValue;
        }
    }
}
