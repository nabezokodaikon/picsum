using SkiaSharp;
using SWF.Core.ImageAccessor;
using SWF.Core.ResourceAccessor;
using System.Collections.Generic;

namespace PicSum.UIComponent.InfoPanel
{
    public static class InfoPanelResources
    {
        public static readonly SKPaint BACKGROUND_PAINT = new()
        {
            Color = new SKColor(250, 250, 250),
            BlendMode = SKBlendMode.Src,
        };

        public static readonly SKPaint IMAGE_PAINT = new()
        {
            IsAntialias = false,
            BlendMode = SKBlendMode.SrcOver,
        };

        public static readonly SKPaint MESSAGE_PAINT = new()
        {
            Color = new(0, 0, 0),
        };

        public static readonly SKPaint ICON_PAINT = new()
        {
            IsAntialias = false,
            BlendMode = SKBlendMode.SrcOver,
        };

        private static readonly Dictionary<float, SKImage> TAG_ICON_CACHE = [];

        public static SKImage GetTagIcon(float scale)
        {
            if (TAG_ICON_CACHE.TryGetValue(scale, out var icon))
            {
                return icon;
            }

            using var surface = SKSurface.Create(new SKImageInfo(
                (int)(ResourceFiles.TagIcon.Value.Width * scale),
                (int)(ResourceFiles.TagIcon.Value.Height * scale)));
            using var canvas = surface.Canvas;
            using var img = SkiaUtil.ToSKImage(ResourceFiles.TagIcon.Value);
            canvas.DrawImage(img, SKRect.Create(0, 0, img.Width, img.Height));
            var newTagIcon = surface.Snapshot();
            TAG_ICON_CACHE.Add(scale, newTagIcon);

            return newTagIcon;
        }

        public static void Dispose()
        {
            BACKGROUND_PAINT.Dispose();
            IMAGE_PAINT.Dispose();
            MESSAGE_PAINT.Dispose();
            ICON_PAINT.Dispose();

            foreach (var item in TAG_ICON_CACHE)
            {
                item.Value.Dispose();
            }

            TAG_ICON_CACHE.Clear();
        }
    }
}
