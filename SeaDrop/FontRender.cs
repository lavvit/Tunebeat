using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SeaDrop
{
    public class FontRender
    {
        #region Command
        /// <summary>
        /// フォント描画用の画像を作成します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="fontName">使用するフォント名</param>
        /// <param name="size">大きさ</param>
        /// <param name="interval">間隔</param>
        /// <param name="foreColor">フォントの色</param>
        /// <param name="space">余白</param>
        /// <returns></returns>
        public static Texture GetTexture(string text, int size, string fontName = "", int interval = 0, int space = 16)
        {
            return GetTexture(text, fontName, size, 0, Color.Black, Color.White, interval, space);
        }
        /// <summary>
        /// フォント描画用の画像を作成します。(縁付き)
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="fontName">使用するフォント名</param>
        /// <param name="size">大きさ</param>
        /// <param name="edge">縁の広さ</param>
        /// <param name="interval">間隔</param>
        /// <param name="foreColor">フォントの色</param>
        /// <param name="backColor">縁の色</param>
        /// <param name="space">余白</param>
        /// <returns></returns>
        public static Texture GetTexture(string text, int size, int edge, string fontName = "", int interval = 0, int space = 16)
        {
            return GetTexture(text, fontName, size, edge, Color.Black, Color.White, interval, space);
        }
        /// <summary>
        /// フォント描画用の画像を作成します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="fontName">使用するフォント名</param>
        /// <param name="size">大きさ</param>
        /// <param name="interval">間隔</param>
        /// <param name="foreColor">フォントの色</param>
        /// <param name="space">余白</param>
        /// <returns></returns>
        public static Texture GetTexture(string text, int size, Color foreColor, string fontName = "", int interval = 0, int space = 16)
        {
            return GetTexture(text, fontName, size, 0, Color.Black, foreColor, interval, space);
        }
        /// <summary>
        /// フォント描画用の画像を作成します。(縁付き)
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="fontName">使用するフォント名</param>
        /// <param name="size">大きさ</param>
        /// <param name="edge">縁の広さ</param>
        /// <param name="interval">間隔</param>
        /// <param name="foreColor">フォントの色</param>
        /// <param name="backColor">縁の色</param>
        /// <param name="space">余白</param>
        /// <returns></returns>
        public static Texture GetTexture(string text, int size, int edge, Color foreColor, string fontName = "", int interval = 0, int space = 16)
        {
            return GetTexture(text, fontName, size, edge, Color.Black, foreColor, interval, space);
        }
        /// <summary>
        /// フォント描画用の画像を作成します。(縁付き)
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="fontName">使用するフォント名</param>
        /// <param name="size">大きさ</param>
        /// <param name="edge">縁の広さ</param>
        /// <param name="interval">間隔</param>
        /// <param name="foreColor">フォントの色</param>
        /// <param name="backColor">縁の色</param>
        /// <param name="space">余白</param>
        /// <returns></returns>
        public static Texture GetTexture(string text, int size, int edge, Color foreColor, Color backColor, string fontName = "", int interval = 0, int space = 16)
        {
            return GetTexture(text, fontName, size, edge, backColor, foreColor, interval, space);
        }
        /// <summary>
        /// フォント描画用の画像を作成します。(縁付き)
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="fontName">使用するフォント名</param>
        /// <param name="size">大きさ</param>
        /// <param name="edge">縁の広さ</param>
        /// <param name="interval">間隔</param>
        /// <param name="foreColor">フォントの色</param>
        /// <param name="backColor">縁の色</param>
        /// <param name="space">余白</param>
        /// <returns></returns>
        public static Texture GetTexture(string text, int size, Color foreColor, Color backColor, int edge = 8, string fontName = "", int interval = 0, int space = 16)
        {
            return GetTexture(text, fontName, size, edge, backColor, foreColor, interval, space);
        }
        /// <summary>
        /// フォント描画用の画像を作成します。(縁付き)
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="fontName">使用するフォント名</param>
        /// <param name="size">大きさ</param>
        /// <param name="edge">縁の広さ</param>
        /// <param name="interval">間隔</param>
        /// <param name="foreColor">フォントの色</param>
        /// <param name="backColor">縁の色</param>
        /// <param name="space">余白</param>
        /// <returns></returns>
        public static Texture GetTexture(string text, int size, int edge, int foreColor, int backColor, string fontName = "", int interval = 0, int space = 16)
        {
            return GetTexture(text, fontName, size, edge, Color.FromArgb(backColor), Color.FromArgb(foreColor), interval, space);
        }
        #endregion
        /// <summary>
        /// フォント描画用の画像を作成します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="fontName">使用するフォント名</param>
        /// <param name="size">大きさ</param>
        /// <param name="edge">縁の広さ</param>
        /// <param name="interval">間隔</param>
        /// <param name="foreColor">フォントの色</param>
        /// <param name="backColor">縁の色</param>
        /// <param name="space">余白</param>
        /// <returns></returns>
        public static Texture GetTexture(string text, string fontName, int size, int edge, Color backColor, Color foreColor, int interval = 0, int space = 16)
        {
            if (string.IsNullOrEmpty(text)) { return new Texture(); }
            if (space < 1) space = 16;

            FontFamily fontFamily = !string.IsNullOrEmpty(fontName) ? new FontFamily(fontName) : new FontFamily("MS UI Gothic");
            List<Bitmap> ListBitmap = new List<Bitmap>();
            List<Bitmap> ListBitmapFore = new List<Bitmap>();

            for (int i = 0; i < text.Length; i++)
            {
                // ※using、Dispose文は省略
                var bitmap = new Bitmap((size + edge + 50), (size + edge + 50));
                // 透過
                bitmap.MakeTransparent();
                // BitmapからGraphicsを生成
                var graphics = Graphics.FromImage(bitmap);

                var stringFormat = new StringFormat();
                // どんなに長くて単語の区切りが良くても改行しない
                stringFormat.FormatFlags = StringFormatFlags.NoWrap;
                // どんなに長くてもトリミングしない
                stringFormat.Trimming = StringTrimming.None;
                // ハイクオリティレンダリング
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                // アンチエイリアスをかける
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                // GraphicsPathを生成
                var gp = new GraphicsPath();

                // パスに文字を追加
                gp.AddString(text[i].ToString(), fontFamily, 0, size, new Point(8, 8), stringFormat);
                // 縁取りをする。
                graphics.DrawPath(new Pen(backColor, edge) { LineJoin = LineJoin.Round }, gp);
                // 文字を塗りつぶす。
                //graphics.FillPath(new SolidBrush(foreColor), gp);

                var bitmapFore = new Bitmap((size + edge + 50), (size + edge + 50));
                var graphicsFore = Graphics.FromImage(bitmapFore);
                graphicsFore.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphicsFore.SmoothingMode = SmoothingMode.HighQuality;
                graphicsFore.FillPath(new SolidBrush(foreColor), gp);

                Rectangle rect;

                if (backColor != Color.Transparent)
                    rect = MeasureForegroundArea(bitmap, true);
                else
                    rect = MeasureForegroundArea(bitmapFore, true);

                ListBitmap.Add(ImageRoi(bitmap, rect));

                ListBitmapFore.Add(ImageRoi(bitmapFore, rect));

                bitmap.Dispose();
                graphics.Dispose();
                gp.Dispose();
            }

            for (int i = 0; i < ListBitmap.Count; i++)
            {
                if (ListBitmap[i] == null)
                    ListBitmap[i] = new Bitmap(space, space);
                if (ListBitmapFore[i] == null)
                    ListBitmapFore[i] = new Bitmap(space, space);
            }

            Size textSize = new Size();
            for (int i = 0; i < text.Length; i++)
            {
                textSize.Width += ListBitmap[i].Width + (i < text.Length - 1 ? interval : 3);
                textSize.Height = ListBitmap[i].Height > textSize.Height ? ListBitmap[i].Height : textSize.Height;
            }

            Bitmap bText = new Bitmap(textSize.Width, textSize.Height);
            Graphics gText = Graphics.FromImage(bText);

            List<int> ListX = new List<int>();
            int x = 0;
            ListX.Add(0);
            for (int i = 0; i < ListBitmap.Count - 1; i++)
            {
                x += ListBitmap[i].Width + interval;
                ListX.Add(x);
            }

            for (int i = ListBitmap.Count - 1; i >= 0; i--)
                gText.DrawImage(ListBitmap[i], new Point(ListX[i], 0));
            for (int i = ListBitmapFore.Count - 1; i >= 0; i--)
                gText.DrawImage(ListBitmapFore[i], new Point(ListX[i], 0));


            Rectangle rText = MeasureForegroundArea(bText, false);

            //Textureクラスに返す
            return new Texture(ImageRoi(bText, rText));
        }

        /// <summary>
        /// Bitmapの一部を切り出したBitmapオブジェクトを返す
        /// </summary>
        /// <param name="srcRect">元のBitmapクラスオブジェクト</param>
        /// <param name="roi">切り出す領域</param>
        /// <returns>切り出したBitmapオブジェクト</returns>
        public static Bitmap ImageRoi(Bitmap src, Rectangle roi)
        {
            //////////////////////////////////////////////////////////////////////
            // srcRectとroiの重なった領域を取得（画像をはみ出した領域を切り取る）

            // 画像の領域
            var imgRect = new Rectangle(0, 0, src.Width, src.Height);
            // はみ出した部分を切り取る(重なった領域を取得)
            var roiTrim = Rectangle.Intersect(imgRect, roi);
            // 画像の外の領域を指定した場合
            if (roiTrim.IsEmpty == true) return null;

            //////////////////////////////////////////////////////////////////////
            // 画像の切り出し

            // 切り出す大きさと同じサイズのBitmapオブジェクトを作成
            var dst = new Bitmap(roiTrim.Width, roiTrim.Height, src.PixelFormat);
            // BitmapオブジェクトからGraphicsオブジェクトの作成
            var g = Graphics.FromImage(dst);
            // 描画先
            var dstRect = new Rectangle(0, 0, roiTrim.Width, roiTrim.Height);
            // 描画
            g.DrawImage(src, dstRect, roiTrim, GraphicsUnit.Pixel);
            // 解放
            g.Dispose();

            return dst;
        }

        /// <summary>
        /// 指定されたBitmapで、backColor以外の色が使われている範囲を計測する
        /// </summary>
        private static Rectangle MeasureForegroundArea(Bitmap bmp, Color backColor, bool notUpDown = false)
        {
            int backColorArgb = backColor.ToArgb();
            int maxWidth = bmp.Width;
            int maxHeight = bmp.Height;

            //左側の空白部分を計測する
            int leftPosition = -1;
            for (int x = 0; x < maxWidth; x++)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    //違う色を見つけたときは、位置を決定する
                    if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
                    {
                        leftPosition = x;
                        break;
                    }
                }
                if (0 <= leftPosition)
                {
                    break;
                }
            }
            //違う色が見つからなかった時
            if (leftPosition < 0)
            {
                return Rectangle.Empty;
            }


            //右側の空白部分を計測する
            int rightPosition = -1;
            for (int x = maxWidth - 1; leftPosition < x; x--)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
                    {
                        rightPosition = x;
                        break;
                    }
                }
                if (0 <= rightPosition)
                {
                    break;
                }
            }
            if (rightPosition < 0)
            {
                rightPosition = leftPosition;
            }

            int topPosition = -1;
            int bottomPosition = -1;
            if (!notUpDown)
            {
                //上の空白部分を計測する
                for (int y = 0; y < maxHeight; y++)
                {
                    for (int x = leftPosition; x <= rightPosition; x++)
                    {
                        if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
                        {
                            topPosition = y;
                            break;
                        }
                    }
                    if (0 <= topPosition)
                    {
                        break;
                    }
                }
                if (topPosition < 0)
                {
                    return Rectangle.Empty;
                }

                //下の空白部分を計測する
                for (int y = maxHeight - 1; topPosition < y; y--)
                {
                    for (int x = leftPosition; x <= rightPosition; x++)
                    {
                        if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
                        {
                            bottomPosition = y;
                            break;
                        }
                    }
                    if (0 <= bottomPosition)
                    {
                        break;
                    }
                }
                if (bottomPosition < 0)
                {
                    bottomPosition = topPosition;
                }
            }


            if (notUpDown)
            {
                //結果を返す
                return new Rectangle(leftPosition, 0,
                    rightPosition - leftPosition + 1, bmp.Height + 2);
            }
            else
            {
                //結果を返す
                return new Rectangle(leftPosition, topPosition,
                    rightPosition - leftPosition + 1, bottomPosition - topPosition + 2);
            }
        }

        private static Rectangle MeasureForegroundArea(Bitmap bmp, bool notUpDown)
        {
            return MeasureForegroundArea(bmp, bmp.GetPixel(0, 0), notUpDown);
        }

    }
}
