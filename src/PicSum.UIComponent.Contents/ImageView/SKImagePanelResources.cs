using SkiaSharp;

namespace PicSum.UIComponent.Contents.ImageView
{
    // TODO: 終了時に破棄
    public static class SKImagePanelResources
    {
        public static readonly SKColor BACKGROUND_COLOR = new(64, 68, 71);

        public static readonly SKPaint IMAGE_PAINT = new()
        {
            IsAntialias = false,
        };

        public static readonly SKPaint INACTIVE_THUMBNAIL_PAINT = new()
        {

        };

        public static readonly SKPaint ACTIVE_THUMBNAIL_PAINT = new()
        {

        };

        public static readonly SKPaint THUMBNAI_PANEL_PAINT = new()
        {
            Color = new(128, 128, 128, 128),
        };

        public static readonly SKPaint THUMBNAI_FILTER_PAINT = new()
        {
            Color = new(0, 0, 0, 128),
        };

        public static readonly SKPaint ACTIVE_THUMBNAI_STROKE_PAINT = new()
        {
            Color = new(255, 0, 0, 255),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        public static readonly SKPaint MESSAGE_PAINT = new()
        {
            Color = new(192, 192, 192),
        };

        public static readonly SKPaint LOADING_PAINT = new()
        {
            Color = new(128, 128, 128),
        };
    }
}
