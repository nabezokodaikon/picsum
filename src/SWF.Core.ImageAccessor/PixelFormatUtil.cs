using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SWF.Core.FileAccessor;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class PixelFormatUtil
    {
        public static bool IsAlpha(string filePath)
        {
            var sw = Stopwatch.StartNew();

            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
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
            finally
            {
                sw.Stop();
                Console.WriteLine($"IsAlpha: {sw.ElapsedMilliseconds} ms");
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
