using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Runtime.Versioning;

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
                    var info = Image.Identify(filePath);
                    return IsAlphaByPng(info);
                }
                catch (NotSupportedException e)
                {
                    throw new ImageUtilException(CreateErrorMessage(filePath), e);
                }
                catch (InvalidImageContentException e)
                {
                    throw new ImageUtilException(CreateErrorMessage(filePath), e);
                }
                catch (UnknownImageFormatException e)
                {
                    throw new ImageUtilException(CreateErrorMessage(filePath), e);
                }
            }
            else
            {
                return false;
            }

        }

        private static bool IsAlphaByPng(ImageInfo info)
        {
            if (info.Metadata.TryGetPngMetadata(out PngMetadata metadata))
            {
                if (metadata.ColorType == PngColorType.RgbWithAlpha)
                {
                    return true;
                }
                else if (metadata.ColorType == PngColorType.GrayscaleWithAlpha)
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

        private static string CreateErrorMessage(string path)
        {
            return $"'{path}'のピクセル情報を取得できませんでした。";
        }
    }
}
