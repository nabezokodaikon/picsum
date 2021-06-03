using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SWF.UIComponent.FreeForm.Properties;

namespace SWF.UIComponent.FreeForm
{
    internal class StandardFrame
    {
        private readonly int _frameSize;
        private readonly List<SolidBrush> _frameBrushList = new List<SolidBrush>();
        private readonly Bitmap _leftTopImage;
        private readonly Bitmap _leftBottomImage;
        private readonly Bitmap _rightTopImage;
        private readonly Bitmap _rightBottomImage;

        public int FrameSize
        {
            get
            {
                return _frameSize;
            }
        }

        public IList<SolidBrush> FrameBrushList
        {
            get
            {
                return _frameBrushList;
            }
        }

        public Bitmap LeftTopImage
        {
            get
            {
                return _leftTopImage;
            }
        }

        public Bitmap LeftBottomImage
        {
            get
            {
                return _leftBottomImage;
            }
        }

        public Bitmap RightTopImage
        {
            get
            {
                return _rightTopImage;
            }
        }

        public Bitmap RightBottomImage
        {
            get
            {
                return _rightBottomImage;
            }
        }

        public StandardFrame()
        {
            Bitmap cornerImage = Resources.Corner;
            Bitmap edgeImage = Resources.Edge;

            if (cornerImage.Width != cornerImage.Height)
            {
                throw new Exception("Cornerイメージが、正方形ではありません。");
            }

            if (cornerImage.Height != edgeImage.Height)
            {
                throw new Exception("CornerイメージとEdgeイメージの高さが一致していません。");
            }

            _frameSize = edgeImage.Height;

            for (int y = 0; y < edgeImage.Height; y++)
            {
                _frameBrushList.Add(new SolidBrush(edgeImage.GetPixel(0, y)));
            }

            using (Bitmap bmp = getBitmapClone(cornerImage))
            {
                _leftTopImage = getBitmapClone(bmp);
                _rightTopImage = getRotateFlip(bmp, RotateFlipType.Rotate90FlipNone);
                _rightBottomImage = getRotateFlip(bmp, RotateFlipType.Rotate180FlipNone);
                _leftBottomImage = getRotateFlip(bmp, RotateFlipType.Rotate270FlipNone);
            }
        }

        public Region GetRegion(int width, int height)
        {
            GraphicsPath path = new GraphicsPath();

            path.AddRectangles(new Rectangle[] { getLeftRectangle(height),
                                                 getTopRectangle(width), 
                                                 getRightRectangle(width, height), 
                                                 getBottomRectangle(width, height),                                                  
                                                 getContentsRectangle(width, height) });

            // 左上
            setPath(ref path, _leftTopImage, getLeftTopRectangle());

            // 右上
            setPath(ref path, _rightTopImage, getRightTopRectangle(width));

            //// 右下
            setPath(ref path, _rightBottomImage, getRightBottomRectangle(width, height));

            // 左下
            setPath(ref path, _leftBottomImage, getLeftBottomRectangle(width, height));

            Region region = new Region(path);
            path.Dispose();

            return region;
        }

        public Rectangle GetLeftRectangle(int height)
        {
            return getLeftRectangle(height);
        }

        public Rectangle GetTopRectangle(int width)
        {
            return getTopRectangle(width);
        }

        public Rectangle GetRightRectangle(int width, int height)
        {
            return getRightRectangle(width, height);
        }

        public Rectangle GetBottomRectangle(int width, int height)
        {
            return getBottomRectangle(width, height);
        }

        public Rectangle GetLeftTopRectangle()
        {
            return getLeftTopRectangle();
        }

        public Rectangle GetRightTopRectangle(int width)
        {
            return getRightTopRectangle(width);
        }

        public Rectangle GetRightBottomRectangle(int width, int height)
        {
            return getRightBottomRectangle(width, height);
        }

        public Rectangle GetLeftBottomRectangle(int width, int height)
        {
            return getLeftBottomRectangle(width, height);
        }

        public Rectangle GetContentsRectangle(int width, int height)
        {
            return getContentsRectangle(width, height);
        }

        private void setPath(ref GraphicsPath path, Bitmap bmp, Rectangle rect)
        {
            int transparent = Color.Transparent.ToArgb();

            for (int i = rect.Y; i < rect.Bottom; i++)
            {
                int y = i - rect.Y;
                for (int j = rect.X; j < rect.Right; j++)
                {
                    int x = j - rect.X;

                    if (bmp.GetPixel(x, y).ToArgb() != transparent)
                    {
                        int xStart = j;
                        while ((j < rect.Right) && (bmp.GetPixel(j - rect.X, y).ToArgb() != transparent))
                        {
                            j++;
                        }

                        path.AddRectangle(new Rectangle(xStart, i, j - xStart, 1));
                    }
                }
            }
        }

        private Bitmap getBitmapClone(Bitmap bmp)
        {
            return (Bitmap)bmp.Clone();
        }

        private Bitmap getRotateFlip(Bitmap srcBmp, RotateFlipType rotateFlipType)
        {
            Bitmap newBmp = getBitmapClone(srcBmp);
            newBmp.RotateFlip(rotateFlipType);
            return newBmp;
        }

        #region 領域取得

        private Rectangle getLeftRectangle(int height)
        {
            return new Rectangle(0, _frameSize, _frameSize, height - _frameSize * 2);
        }

        private Rectangle getTopRectangle(int width)
        {
            return new Rectangle(_frameSize, 0, width - _frameSize * 2, _frameSize);
        }

        private Rectangle getRightRectangle(int width, int height)
        {
            return new Rectangle(width - _frameSize, _frameSize, _frameSize, height - _frameSize * 2);
        }

        private Rectangle getBottomRectangle(int width, int height)
        {
            return new Rectangle(_frameSize, height - _frameSize, width - _frameSize * 2, _frameSize);
        }

        private Rectangle getLeftTopRectangle()
        {
            return new Rectangle(0, 0, _frameSize, _frameSize);
        }

        private Rectangle getRightTopRectangle(int width)
        {
            return new Rectangle(width - _frameSize, 0, _frameSize, _frameSize);
        }

        private Rectangle getRightBottomRectangle(int width, int height)
        {
            return new Rectangle(width - _frameSize, height - _frameSize, _frameSize, _frameSize);
        }

        private Rectangle getLeftBottomRectangle(int width, int height)
        {
            return new Rectangle(0, height - _frameSize, _frameSize, _frameSize);
        }

        private Rectangle getContentsRectangle(int width, int height)
        {
            return new Rectangle(_frameSize, _frameSize, width - _frameSize * 2, height - _frameSize * 2);
        }

        #endregion
    }
}
