using System;
using System.Runtime.Versioning;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp;

namespace SWF.Common
{
    [SupportedOSPlatform("windows")]
    public static class PixelFormatUtil
    {
        public static bool IsAlpha(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtension(filePath);
            if (ex == ".PNG")
            {
                try
                {
                    var info = SixLabors.ImageSharp.Image.Identify(filePath);
                    return IsAlphaByPng(info);
                }
                catch (NotSupportedException e)
                {
                    throw new ImageUtilException(CreateErrorMessage(filePath), e);
                }
                catch (SixLabors.ImageSharp.InvalidImageContentException e)
                {
                    throw new ImageUtilException(CreateErrorMessage(filePath), e);
                }
                catch (SixLabors.ImageSharp.UnknownImageFormatException e)
                {
                    throw new ImageUtilException(CreateErrorMessage(filePath), e);
                }
            }
            else
            {
                return false;
            }
        }

        private static bool IsAlphaByPng(SixLabors.ImageSharp.ImageInfo info)
        {
            if (info.Metadata.TryGetPngMetadata(out SixLabors.ImageSharp.Formats.Png.PngMetadata metadata))
            {
                if (metadata.ColorType == SixLabors.ImageSharp.Formats.Png.PngColorType.RgbWithAlpha)
                {
                    return true;
                }
                else if (metadata.ColorType == SixLabors.ImageSharp.Formats.Png.PngColorType.GrayscaleWithAlpha)
                {
                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        public static string CreateErrorMessage(string path)
        {
            return $"'{path}'のピクセル情報を取得できませんでした。";
        }
    }
}
